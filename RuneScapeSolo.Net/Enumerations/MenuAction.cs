namespace RuneScapeSolo.Net.Enumerations
{
    public enum MenuAction
    {
        CastSpellOnGroundItem = 200,
        UseItemWithGroundItem = 210,
        TakeItem = 220,
        CastSpellOnWallObject = 300,
        UseItemWithWallObject = 310,
        Command1OnWallObject = 320,
        CastSpellOnModel = 400,
        UseItemWithModel = 410,
        Command1OnModel = 420,
        CastSpellOnItem = 600,
        UseItemWithItem = 610,
        RemoveItem = 620,
        EquipItem = 630,
        CommandOnItem = 640,
        UseItem = 650,
        DropItem = 660,
        CastSpellOnNpc = 700,
        UseItemWithNpc = 710,
        AttackNpc = 715, // TODO: Find the difference between this and the other
        TalkToNpc = 720,
        CommandOnNpc = 725,
        CastSpellOnGround = 900,
        WalkHere = 920,
        CastSpellOnSelf = 1000,
        Command2OnWallObject = 2300,
        Command2OnModel = 2400,
        AttackNpc2 = 2715, // TODO: Find the difference between this and the other
        AddFriend = 2830,
        ExamineGroundItem = 3200,
        ExamineWallObject = 3300,
        ExamineModel = 3400,
        ExamineItem = 3600,
        ExamineNpc = 3700,
        Cancel = 4000,
        TeleportHere = 9902,
    }
}
