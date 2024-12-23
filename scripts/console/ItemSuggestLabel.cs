﻿using Godot;

namespace ColdMint.scripts.console;

/// <summary>
/// <para>ItemSuggestLabel</para>
/// <para>输入建议标签</para>
/// </summary>
public partial class ItemSuggestLabel : RichTextLabel
{
    public LineEdit? LineEdit;

    public InputSuggestion SuggestValue;

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (LineEdit == null)
        {
            return;
        }

        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            var inputText = LineEdit.Text;
            if (string.IsNullOrEmpty(inputText))
            {
                SetLineEditText(SuggestValue.Value);
                return;
            }

            var index = inputText.LastIndexOf(' ');
            if (index == -1)
            {
                SetLineEditText(SuggestValue.Value);
                return;
            }

            if (index == inputText.Length - 1)
            {
                SetLineEditText(inputText + SuggestValue.Value);
                return;
            }

            SetLineEditText(inputText[..index] +" "+ SuggestValue.Value);
        }
    }

    private void SetLineEditText(string text)
    {
        if (LineEdit == null)
        {
            return;
        }

        LineEdit.Text = text + " ";
        LineEdit.EmitSignal("text_changed", text);
        LineEdit.CaretColumn = text.Length + 1;
    }
}