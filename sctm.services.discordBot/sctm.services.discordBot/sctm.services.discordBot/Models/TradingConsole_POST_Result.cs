using sctm.connectors.sctmDB.Models.DBModels;
using sctm.connectors.sctmDB.Models.Models.RSI;
using sctm.connectors.sctmDB.Models.Models.Screenshots;
using sctm.connectors.sctmDB.Models.Models.Screenshots.OCR.TradeConsole;
using System;
using System.Collections.Generic;

namespace sctm.services.discordBot.Models
{
    public class TradingConsole_POST_Result
    {
        public ImageUpload Image { get; set; }
        public object DatabaseRecord { get; set; }
    }

    public class ImageUpload
    {
        public string S3Id { get; set; }
        public string ImageUrl { get; set; }

        public string JsonUrl { get; set; }

        public ProcessImage_Result Data { get; set; }

        public string MD5Hash { get; set; }
    }

    public class ProcessImage_Result
    {
        public ScreenShotTypes ScreenshotType { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }

    public class AddConfirmScreen_Result
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public RecordStatus RecordStatus { get; set; }
        public int CreateScreenId { get; set; }

        public int Units { get; set; }
        public string Item { get; set; }
        public int TotalTransactionCost { get; set; }

        public int? Wallet { get; set; }
        public TransactionTypes TransactionType { get; set; }


        public string ShipIdentifier { get; set; }
        public int TotalCargoSpace { get; set; }
        public int EmptyCargoSpace { get; set; }

        public CargoUnitsOfMeasure CargoUnitOfMeasure { get; set; }

        public string JsonUrl { get; set; }

        public string UserIdentifier { get; set; }
        public string TeamIdentifier { get; set; }
        public string OrganizationIdentifier { get; set; }

        public string ImageUrl { get; set; }
        public string ImageHash { get; set; }

        public string UserNotes { get; set; }

    }

    public class AddCreateScreen_Result
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public RecordStatus RecordStatus { get; set; }

        public List<sctm.connectors.sctmDB.Models.Models.Screenshots.OCR.TradeConsole.LineItem> Items { get; set; }
        public int TotalTransactionValue { get; set; }

        public int? Wallet { get; set; }
        public TransactionTypes TransactionType { get; set; }


        public string ShipIdentifier { get; set; }
        public int TotalCargoSpace { get; set; }
        public int EmptyCargoSpace { get; set; }

        public CargoUnitsOfMeasure CargoUnitOfMeasure { get; set; }

        public string JsonUrl { get; set; }

        public string UserIdentifier { get; set; }
        public string TeamIdentifier { get; set; }
        public string OrganizationIdentifier { get; set; }

        public string ImageUrl { get; set; }
        public string ImageHash { get; set; }

        public string UserNotes { get; set; }
    }
}
