﻿using Maasgroep.Database.Receipts;

namespace Maasgroep.Database.Photos
{
    internal record Photo
    {
        public long Id { get; set; }
        public long? Receipt { get; set; }

        public DateTime Created { get; set; }

        public byte[] Bytes { get; set; }

        public string? Location { get; set; }//TODO: Kevin; GPS zie ik nog even niet vliegen?


        //Ef
        public Receipt? ReceiptInstance { get; set; }

    }
}