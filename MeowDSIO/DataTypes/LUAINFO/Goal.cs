using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.LUAINFO
{
    public class Goal
    {
        public int ID { get; set; } = -1;

        public string Name { get; set; } = null;

        /// <summary>
        /// ALL battle goals have this set to NULL. This is because all battle scripts are expected 
        /// to have _Activate, _Update, _Terminate, and _Interupt
        /// functions included, regardless of them being used or not.
        /// Logic functions, on the other hand, have interrupt functions that don't 
        /// actually begin with the same root word as the goal name
        /// e.g. "ShinenBito415000_Logic" and "ShinenBito415000_Interupt"; if it followed the same
        /// logic as battle scripts, it would look for an interrupt function named "ShinenBito415000_Logic_Interupt".
        /// <para/>
        /// Logic goals with <see cref="IsLogicInterrupt"/> set to FALSE, however, 
        /// will have a NULL value defined for <see cref="LogicInterruptName"/>. 
        /// </summary>
        public string LogicInterruptName { get; set; } = null;

        /// <summary>
        /// Tells game to actually run the "&lt;GoalName&gt;Battle_Interupt" function.
        /// <para/>
        /// For goals inside aiCommon.luabnd like BattleActivate, this flag being set to FALSE almost always 
        /// coincides with a global call to disable the interrupt e.g. 
        /// <code>REGISTER_GOAL_NO_INTERUPT(GOAL_COMMON_BattleActivate, true)</code>
        /// </summary>
        public bool IsBattleInterrupt { get; set; } = true;

        /// <summary>
        /// Tells game to actually run the function with same name as <see cref="LogicInterruptName"/>.
        /// <para/>
        /// For goals inside aiCommon.luabnd like SpecialTurn, this flag being set to FALSE almost always 
        /// coincides with a global call to disable the interrupt e.g. 
        /// <code>REGISTER_GOAL_NO_INTERUPT(GOAL_COMMON_SpecialTurn, true)</code>
        /// <para/>
        /// Additionally, any logic goal without an interrupt has <see cref="LogicInterruptName"/> set to NULL.
        /// </summary>
        public bool IsLogicInterrupt { get; set; } = false;

        public byte Unknown1 { get; set; } = 0;

        public byte Unknown2 { get; set; } = 0;
    }
}
