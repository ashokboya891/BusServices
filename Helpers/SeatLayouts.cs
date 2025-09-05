using BusServices.Models;
// Data/SeatLayouts.cs
using System.Collections.Generic;


namespace BusServices.Helpers
{


    public static class SeatLayouts
    {

        public static SeatLayoutConfig Generate(string busTypeName)
        {
            return busTypeName.ToLower() switch
            {
                "ac 2+2 seater" => Seater(40),
                "ac sleeper" => Sleeper(30),
                "volvo express" => Seater(36),
                "intercity" => Seater(45),
                "garuda" => Sleeper(30),
                "indra" => SemiSleeper(36),
                "express" => Seater(45),
                "deluxe" => SemiSleeper(32),
                "courier(heavy)" => new SeatLayoutConfig { Rows = 0, Cols = 0, Pattern = new() },
                "night sleeper" => Sleeper(30),
                "economy seater" => Seater(50),
                "night express" => SemiSleeper(36),
                _ => new SeatLayoutConfig { Rows = 0, Cols = 0, Pattern = new() }
            };
        }
        // Seater 2+2 (aisle in middle): rows of [S,S,null,S,S]
        public static SeatLayoutConfig Seater(int seats)
        {
            var pattern = new List<List<string?>>();
            int seat = 1;
            int fullRows = seats / 4;               // each full row has 4 seats
            int rem = seats % 4;                    // leftover seats in last row

            for (int r = 0; r < fullRows; r++)
                pattern.Add(new() { (seat++).ToString(), (seat++).ToString(), null, (seat++).ToString(), (seat++).ToString() });

            if (rem > 0)
            {
                // fill leftover seats from left to right
                var row = new List<string?>(new string?[5]);
                if (rem >= 1) row[0] = (seat++).ToString();
                if (rem >= 2) row[1] = (seat++).ToString();
                row[2] = null; // aisle
                if (rem >= 3) row[3] = (seat++).ToString();
                if (rem == 4) row[4] = (seat++).ToString();
                pattern.Add(row);
            }

            return new SeatLayoutConfig { Rows = pattern.Count, Cols = 5, Pattern = pattern };
        }

        // Semi-sleeper 2+1 (aisle after first seat): [S,null,S,S]
        public static SeatLayoutConfig SemiSleeper(int seats)
        {
            var pattern = new List<List<string?>>();
            int seat = 1;
            int fullRows = seats / 3;  // 3 seats per row
            int rem = seats % 3;

            for (int r = 0; r < fullRows; r++)
                pattern.Add(new() { (seat++).ToString(), null, (seat++).ToString(), (seat++).ToString() });

            if (rem > 0)
            {
                var row = new List<string?>(new string?[4]) { null, null, null, null };
                // left window first, then right window, then right aisle-side
                if (rem >= 1) row[0] = (seat++).ToString();
                row[1] = null; // aisle
                if (rem >= 2) row[2] = (seat++).ToString();
                if (rem == 3) row[3] = (seat++).ToString();
                pattern.Add(row);
            }

            return new SeatLayoutConfig { Rows = pattern.Count, Cols = 4, Pattern = pattern };
        }

        // Sleeper: labels L1.., U1.. ; two berths per row on each deck: [X,null,Y]
        //public static SeatLayoutConfig Sleeper(int seats)
        //{
        //    int perDeck = seats / 2;        // split evenly lower/upper
        //    int lower = perDeck, upper = perDeck;

        //    var pattern = new List<List<string?>>();

        //    // LOWER deck rows (two berths per row)
        //    int l = 1;
        //    int lowerFull = lower / 2;
        //    int lowerRem = lower % 2;
        //    for (int r = 0; r < lowerFull; r++)
        //        pattern.Add(new() { $"L{l++}", null, $"L{l++}" });
        //    if (lowerRem > 0)
        //        pattern.Add(new() { $"L{l++}", null, null });

        //    // UPPER deck rows
        //    int u = 1;
        //    int upperFull = upper / 2;
        //    int upperRem = upper % 2;
        //    for (int r = 0; r < upperFull; r++)
        //        pattern.Add(new() { $"U{u++}", null, $"U{u++}" });
        //    if (upperRem > 0)
        //        pattern.Add(new() { $"U{u++}", null, null });

        //    return new SeatLayoutConfig { Rows = pattern.Count, Cols = 3, Pattern = pattern };
        //}

        // Sleeper: labels L1.., U1.. ; paired per column: [U,L,null,U,L]
        public static SeatLayoutConfig Sleeper(int seats)
        {
            int perSide = seats / 2; // total per side (U+L)
            var pattern = new List<List<string?>>();
            int l = 1, u = 1;

            // Each row has U + L left, aisle, U + L right
            int fullRows = perSide / 2; // two columns = left+right
            int rem = perSide % 2;

            for (int r = 0; r < fullRows; r++)
            {
                pattern.Add(new()
        {
            $"U{u++}", $"L{l++}",
            null,
            $"U{u++}", $"L{l++}"
        });
            }

            if (rem > 0)
            {
                var row = new List<string?>(new string?[5]);
                row[0] = $"U{u++}";
                row[1] = $"L{l++}";
                row[2] = null;
                pattern.Add(row);
            }

            return new SeatLayoutConfig { Rows = pattern.Count, Cols = 5, Pattern = pattern };
        }

    }

}
