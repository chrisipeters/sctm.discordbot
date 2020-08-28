using sctm.connectors.sctmDB.Models;
using sctm.connectors.sctmDB.Models.Experience;
using sctm.connectors.sctmDB.Models.RSI;
using sctm.connectors.sctmDB.Models.Screenshots;
using sctm.connectors.sctmDB.Models.Screenshots.OCR.TradeConsole;
using System;
using System.Collections.Generic;

namespace sctm.services.discordBot.Models
{
    public class TradingConsole_POST_Result
    {
        public ScreenShotTypes ImageType { get; set; }
        public object Record { get; set; }

        public ExperienceResult Experience { get; set; }
    }
    public class ExperienceResult
    {
        public AddShipExperience_Result Ship { get; set; }
        public List<AddProfessionExperience_Result> Professions { get; set; }
        public List<AddTaskExperience_Result> Tasks { get; set; }

    }

    public class AddShipExperience_Result
    {
        public bool Success { get; set; } = false;
        public ShipExperience Entry { get; set; }

        public AddShipExperience_Result_Summary Previous { get; set; }
        public AddShipExperience_Result_Summary New { get; set; }
        public string Message { get; set; }
    }

    public class AddShipExperience_Result_Summary
    {
        public ulong ShipXP { get; set; }
        public ulong ProficiencyXP { get; set; }
        public int ShipLevel { get; set; }
        public int ProficiencyLevel { get; set; }
    }

    public class AddProfessionExperience_Result
    {
        public bool Success { get; set; } = false;
        public ProfessionExperience Entry { get; set; }

        public AddProfessionExperience_Result_Summary Previous { get; set; }
        public AddProfessionExperience_Result_Summary New { get; set; }
        public string Message { get; set; }
    }

    public class AddProfessionExperience_Result_Summary
    {
        public ulong ProfessionXP { get; set; }
        public ulong ProficiencyXP { get; set; }
        public int ProfessionLevel { get; set; }
        public int ProficiencyLevel { get; set; }
    }

    public class AddTaskExperience_Result
    {
        public bool Success { get; set; } = false;
        public TaskExperience Entry { get; set; }

        public AddTaskExperience_Result_Summary Previous { get; set; }
        public AddTaskExperience_Result_Summary New { get; set; }
        public string Message { get; set; }
    }

    public class AddTaskExperience_Result_Summary
    {
        public ulong TaskXP { get; set; }
        public int TaskLevel { get; set; }
    }

    public class AddCreateScreen_Record
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public RecordStatus RecordStatus { get; set; }

        public List<LineItem> Items { get; set; }
        public int TotalTransactionValue { get; set; }

        public TransactionTypes TransactionType { get; set; }


        public string ShipIdentifier { get; set; }
        public int TotalCargoSpace { get; set; }
        public int EmptyCargoSpace { get; set; }

        public CargoUnitsOfMeasure CargoUnitOfMeasure { get; set; }

        public string UserIdentifier { get; set; }
        public string TeamIdentifier { get; set; }
        public string OrganizationIdentifier { get; set; }

        public string ImageUrl { get; set; }
        public string ImageHash { get; set; }

        public string UserNotes { get; set; }
    }


    public class AddConfirmScreen_Record
    {
        public int Id { get; set; }
        public int Units { get; set; }
        public string Item { get; set; }
        public int TotalTransactionCost { get; set; }
        public TransactionTypes TransactionType { get; set; }

        public DateTime Created { get; set; }
        public int? CreateScreenId { get; set; }

        public string ShipIdentifier { get; set; }
        public int TotalCargoSpace { get; set; }
        public int EmptyCargoSpace { get; set; }

        public CargoUnitsOfMeasure CargoUnitOfMeasure { get; set; }
        public string UserIdentifier { get; set; }
        public string TeamIdentifier { get; set; }
        public string OrganizationIdentifier { get; set; }

        public string ImageUrl { get; set; }

        public string UserNotes { get; set; }
    }


}
