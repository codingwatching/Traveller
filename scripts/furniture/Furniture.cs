using System;
using ColdMint.scripts.damage;
using ColdMint.scripts.loot;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.furniture;

/// <summary>
/// <para>FurnitureTemplate</para>
/// <para>家具模板</para>
/// </summary>
public partial class Furniture : RigidBody2D
{
    [Export] private int _initialDurability; // skipcq:CS-R1137
    [Export] private int _maxDurability; // skipcq:CS-R1137
    [Export] private string? _furnitureName; // skipcq:CS-R1137

    [Export] private string? _lootId; // skipcq:CS-R1137

    private Label? _tipLabel;
    private AudioStreamPlayer2D? _audioStreamPlayer2D;
    private CollisionShape2D? _collisionShape2D;

    public override void _MouseEnter()
    {
        if (_tipLabel == null || string.IsNullOrEmpty(_furnitureName))
        {
            return;
        }

        var translation = TranslationServerUtils.Translate(_furnitureName);
        if (string.IsNullOrEmpty(translation))
        {
            return;
        }

        TipLabelUtils.ShowTip(0, _tipLabel, translation, Colors.White);
    }

    public override void _MouseExit()
    {
        if (_tipLabel == null)
        {
            return;
        }

        TipLabelUtils.HideTip(_tipLabel);
    }

    /// <summary>
    /// <para></para>
    /// <para>家具的耐久度</para>
    /// </summary>
    private int _durability;

    public override void _Ready()
    {
        if (_maxDurability <= 0)
        {
            _maxDurability = Config.DefaultMaxDurability;
        }

        if (_initialDurability <= 0 || _initialDurability > _maxDurability)
        {
            _initialDurability = _maxDurability;
        }

        _tipLabel = GetNodeOrNull<Label>("TipLabel");
        _collisionShape2D = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        _audioStreamPlayer2D = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        InputPickable = true;
        _durability = _initialDurability;
        SetCollisionLayerValue(Config.LayerNumber.Furniture, true);
        SetCollisionMaskValue(Config.LayerNumber.Wall, true);
        SetCollisionMaskValue(Config.LayerNumber.Platform, true);
        SetCollisionMaskValue(Config.LayerNumber.Floor, true);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased("push"))
        {
            SetCollisionMaskValue(Config.LayerNumber.Player, false);
        }

        if (Input.IsActionJustPressed("push"))
        {
            SetCollisionMaskValue(Config.LayerNumber.Player, true);
        }
    }

    /// <summary>
    /// <para>OnDestroy</para>
    /// <para>当家具被破坏时</para>
    /// </summary>
    private void OnDestroy()
    {
        GenerateLoot(QueueFree);
    }

    /// <summary>
    /// <para>Loot is generated when furniture is destroyed</para>
    /// <para>家具被破坏时生成战利品</para>
    /// </summary>
    private void GenerateLoot(Action onCompleted)
    {
        if (string.IsNullOrEmpty(_lootId))
        {
            return;
        }

        LootListManager
            .GenerateLootObjectsAsync<Node2D>(GetParent(), LootListManager.GenerateLootData(_lootId),
                GlobalPosition - new Vector2(0, Config.CellSize * 2)).GetAwaiter().OnCompleted(onCompleted);
    }

    /// <summary>
    /// <para>This method is called when furniture is damaged</para>
    /// <para>当家具损害时调用此方法</para>
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>
    ///<para>Return whether the damage completely destroyed the furniture</para>
    ///<para>返回本次伤害是否彻底破坏了家具</para>
    /// </returns>
    public bool Damage(IDamage damage)
    {
        if (_durability <= 0)
        {
            //Do not damage broken furniture twice.
            //不能对已破碎的家具二次伤害。
            return false;
        }

        _durability -= damage.Damage;
        if (_durability <= 0)
        {
            if (_audioStreamPlayer2D == null)
            {
                OnDestroy();
            }
            else
            {
                //If there is a sound effect, we wait for the sound effect to play and then destroy the node.
                //如果有音效，我们等待音效播放完毕后销毁节点。
                _audioStreamPlayer2D.Finished += OnDestroy;
                _audioStreamPlayer2D.Play();
                //Disable collisions and hide nodes in order to make the player appear destroyed.
                //禁用碰撞，隐藏节点，以便让玩家看起来被销毁了。
                if (_collisionShape2D != null)
                {
                    _collisionShape2D.Disabled = true;
                }

                Hide();
            }
        }

        return true;
    }
}