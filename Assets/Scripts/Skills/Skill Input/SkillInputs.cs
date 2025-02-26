using System.Collections.Generic;
using System.Linq;

public static class SkillInputs
{
    private static readonly SkillInput[] l3Input =
    {
        SkillInput.L3_Right,
        SkillInput.L3_UpRight,
        SkillInput.L3_Up,
        SkillInput.L3_UpLeft,
        SkillInput.L3_Left,
        SkillInput.L3_DownLeft,
        SkillInput.L3_Down,
        SkillInput.L3_DownRight,

        SkillInput.Hold_L3_Any,

        SkillInput.L3_None
    };

    private static readonly SkillInput[] r3Input =
    {
        SkillInput.R3_Right,
        SkillInput.R3_UpRight,
        SkillInput.R3_Up,
        SkillInput.R3_UpLeft,
        SkillInput.R3_Left,
        SkillInput.R3_DownLeft,
        SkillInput.R3_Down,
        SkillInput.R3_DownRight,

        SkillInput.Hold_R3_Any,

        SkillInput.R3_None
    };

    private static readonly SkillInput[] R3_RightToUp = { SkillInput.R3_Right, SkillInput.R3_Up };
    private static readonly SkillInput[] R3_RightToDown = { SkillInput.R3_Right, SkillInput.R3_Down };

    private static readonly SkillInput[] R3_UpToRight = { SkillInput.R3_Up, SkillInput.R3_Right };
    private static readonly SkillInput[] R3_UpToLeft = { SkillInput.R3_Right, SkillInput.R3_Left };

    private static readonly SkillInput[] R3_DownToRight = { SkillInput.R3_Down, SkillInput.R3_Right };
    private static readonly SkillInput[] R3_DownToLeft = { SkillInput.R3_Down, SkillInput.R3_Left };

    private static readonly SkillInput[] R3_LeftToUp = { SkillInput.R3_Left, SkillInput.R3_Up };
    private static readonly SkillInput[] R3_LeftToDown = { SkillInput.R3_Left, SkillInput.R3_Down };
    private static readonly SkillInput[] R3_LeftToDownToLeft = { SkillInput.R3_Left, SkillInput.R3_Down, SkillInput.R3_Left };
    private static readonly SkillInput[] R3_LeftToUpToLeft = { SkillInput.R3_Left, SkillInput.R3_Up, SkillInput.R3_Left };


    public static SkillInput? GetTabInput(string buttonName)
    {
        return (buttonName) switch
        {
            ("buttonSouth") => SkillInput.Button_X,
            ("buttonEast") => SkillInput.Button_Circle,
            ("buttonWest") => SkillInput.Button_Square,
            ("buttonNorth") => SkillInput.Button_Triangle,

            ("leftShoulder") => SkillInput.L1,
            ("rightShoulder") => SkillInput.R1,
            _ => null
        };
    }

    public static SkillInput? GetHoldInput(string buttonName)
    {
        return (buttonName) switch
        {
            ("buttonSouth") => SkillInput.Hold_Button_X,
            ("buttonEast") => SkillInput.Hold_Button_Circle,
            ("buttonWest") => SkillInput.Hold_Button_Square,
            ("buttonNorth") => SkillInput.Hold_Button_Triangle,

            ("leftShoulder") => SkillInput.L1_Hold,
            ("rightShoulder") => SkillInput.R1_Hold,
            _ => null
        };
    }

    public static SkillInput? GetStickInput(string buttonName, bool isLeft = true, bool isHeld = false)
    {
        if (isLeft)
        {
            if (!isHeld)
                return (buttonName) switch
                {
                    ("up") => SkillInput.L3_Up,
                    ("right") => SkillInput.L3_Right,
                    ("down") => SkillInput.L3_Down,
                    ("left") => SkillInput.L3_Left,
                    _ => null
                };
            else
            {
                return (buttonName) switch
                {
                    ("up") => SkillInput.Hold_L3_Up,
                    ("right") => SkillInput.Hold_L3_Right,
                    ("down") => SkillInput.Hold_L3_Down,
                    ("left") => SkillInput.Hold_L3_Left,
                    _ => null
                };
            }
        }
        else
        {
            if (!isHeld)
            {
                return (buttonName) switch
                {
                    ("up") => SkillInput.R3_Up,
                    ("right") => SkillInput.R3_Right,
                    ("down") => SkillInput.R3_Down,
                    ("left") => SkillInput.R3_Left,
                    _ => null
                };
            }
            else
            {
                return (buttonName) switch
                {
                    ("up") => SkillInput.Hold_R3_Up,
                    ("right") => SkillInput.Hold_R3_Right,
                    ("down") => SkillInput.Hold_R3_Down,
                    ("left") => SkillInput.Hold_R3_Left,
                    _ => null
                };

            }
        }
    }


    public static SkillInput? GetStickRotationInput(string buttonName, List<SkillInput> input, UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
        {
            if (buttonName.Contains("rightStick"))
            {
                if (input.SequenceEqual(R3_RightToUp))
                    return SkillInput.R3_RightToUp;

                if (input.SequenceEqual(R3_RightToDown))
                    return SkillInput.R3_RightToDown;

                if (input.SequenceEqual(R3_UpToRight))
                    return SkillInput.R3_UpToRight;

                if (input.SequenceEqual(R3_UpToLeft))
                    return SkillInput.R3_UpToLeft;

                if (input.SequenceEqual(R3_DownToRight))
                    return SkillInput.R3_DownToRight;

                if (input.SequenceEqual(R3_DownToLeft))
                    return SkillInput.R3_DownToLeft;

                if (input.SequenceEqual(R3_LeftToUp))
                    return SkillInput.R3_LeftToUp;

                if (input.SequenceEqual(R3_LeftToDown))
                    return SkillInput.R3_LeftToDown;

                if (input.SequenceEqual(R3_LeftToDownToLeft))
                    return SkillInput.R3_LeftToDownToLeft;

                if (input.SequenceEqual(R3_LeftToUpToLeft))
                    return SkillInput.R3_LeftToUpToLeft;
            }
        }

        return null;
    }

    //TODO: Rework
    //public static SkillInput? GetSkillStickInput(float angle, float magnitude, bool isLeftStick = true, float tolerance = -1)
    //{
    //    SkillInput[] input = isLeftStick ? l3Input : r3Input;

    //    if (magnitude == 0)
    //        return input[8];

    //    if (tolerance <= 0f)
    //    {
    //        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
    //        if (snappedAngle >= 360f)
    //            snappedAngle = 0f;

    //        if (snappedAngle == 0f)
    //            return input[0];                //Right
    //        else if (snappedAngle == 45f)
    //            return input[1];                //Up Right
    //        else if (snappedAngle == 90f)
    //            return input[2];                //Up
    //        else if (snappedAngle == 135f)
    //            return input[3];                //Up Left
    //        else if (snappedAngle == 180f)
    //            return input[4];                //Left
    //        else if (snappedAngle == 225f)
    //            return input[5];                //Down left
    //        else if (snappedAngle == 270f)
    //            return input[6];                //Down
    //        else
    //            return input[7];                //Down Right
    //    }
    //    else
    //    {
    //        if (angle >= (360f - tolerance) || angle < (0f + tolerance))
    //            return input[0];
    //        else if (angle >= (45f - tolerance) && angle < (45f + tolerance))
    //            return input[1];
    //        else if (angle >= (90f - tolerance) && angle < (90f + tolerance))
    //            return input[2];
    //        else if (angle >= (135f - tolerance) && angle < (135f + tolerance))
    //            return input[3];
    //        else if (angle >= (180f - tolerance) && angle < (180f + tolerance))
    //            return input[4];
    //        else if (angle >= (225f - tolerance) && angle < (225f + tolerance))
    //            return input[5];
    //        else if (angle >= (270f - tolerance) && angle < (270f + tolerance))
    //            return input[6];
    //        else if (angle >= (315f - tolerance) && angle < (315f + tolerance))
    //            return input[7];
    //        else
    //            return input[8];
    //    }
    //}
}
