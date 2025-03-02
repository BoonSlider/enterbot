namespace Player.House;

public class Houses
{
    public static HouseData GetHouseData(long houseLevel)
    {
        return houseLevel switch
        {
            1 => new HouseData
                { Price = 100_000, Taxes = 10, ProtectedGuards = 25, MoonshineLimit = 25_000, RequiredFame = 0, },
            2 => new HouseData
                { Price = 905_000, Taxes = 95, ProtectedGuards = 43, MoonshineLimit = 40_000, RequiredFame = 0, },
            3 => new HouseData
                { Price = 1_900_000, Taxes = 190, ProtectedGuards = 65, MoonshineLimit = 70_000, RequiredFame = 0, },
            4 => new HouseData
                { Price = 3_200_000, Taxes = 320, ProtectedGuards = 110, MoonshineLimit = 90_000, RequiredFame = 0, },
            5 => new HouseData
                { Price = 4_610_000, Taxes = 461, ProtectedGuards = 225, MoonshineLimit = 120_000, RequiredFame = 0, },
            6 => new HouseData
                { Price = 6_000_000, Taxes = 600, ProtectedGuards = 290, MoonshineLimit = 140_000, RequiredFame = 0, },
            7 => new HouseData
                { Price = 8_600_000, Taxes = 860, ProtectedGuards = 350, MoonshineLimit = 190_000, RequiredFame = 0, },
            8 => new HouseData
            {
                Price = 10_900_000, Taxes = 1_900, ProtectedGuards = 445, MoonshineLimit = 230_000, RequiredFame = 0,
            },
            9 => new HouseData
            {
                Price = 30_500_000, Taxes = 3_500, ProtectedGuards = 600, MoonshineLimit = 270_000, RequiredFame = 0,
            },
            10 => new HouseData
            {
                Price = 60_000_000, Taxes = 6_000, ProtectedGuards = 725, MoonshineLimit = 330_000, RequiredFame = 1,
            },
            11 => new HouseData
            {
                Price = 90_000_000, Taxes = 9_000, ProtectedGuards = 896, MoonshineLimit = 380_000, RequiredFame = 2,
            },
            12 => new HouseData
            {
                Price = 120_000_000, Taxes = 12_000, ProtectedGuards = 2_000, MoonshineLimit = 420_000,
                RequiredFame = 3,
            },
            13 => new HouseData
            {
                Price = 180_000_000, Taxes = 18_000, ProtectedGuards = 3_250, MoonshineLimit = 680_000,
                RequiredFame = 4,
            },
            14 => new HouseData
            {
                Price = 240_000_000, Taxes = 24_000, ProtectedGuards = 4_000, MoonshineLimit = 820_000,
                RequiredFame = 5,
            },
            15 => new HouseData
            {
                Price = 300_000_000, Taxes = 30_000, ProtectedGuards = 6_000, MoonshineLimit = 1_100_000,
                RequiredFame = 10,
            },
            16 => new HouseData
            {
                Price = 420_000_000, Taxes = 42_000, ProtectedGuards = 7_000, MoonshineLimit = 1_342_000,
                RequiredFame = 20,
            },
            17 => new HouseData
            {
                Price = 540_000_000, Taxes = 54_000, ProtectedGuards = 8_250, MoonshineLimit = 1_502_000,
                RequiredFame = 30,
            },
            18 => new HouseData
            {
                Price = 660_000_000, Taxes = 66_000, ProtectedGuards = 9_000, MoonshineLimit = 1_742_000,
                RequiredFame = 40,
            },
            19 => new HouseData
            {
                Price = 900_000_000, Taxes = 90_000, ProtectedGuards = 10_000, MoonshineLimit = 2_000_000,
                RequiredFame = 50,
            },
            20 => new HouseData
            {
                Price = 1_200_000_000, Taxes = 120_000, ProtectedGuards = 11_500, MoonshineLimit = 2_200_000,
                RequiredFame = 75,
            },
            21 => new HouseData
            {
                Price = 1_500_000_000, Taxes = 150_000, ProtectedGuards = 13_000, MoonshineLimit = 2_500_000,
                RequiredFame = 100,
            },
            22 => new HouseData
            {
                Price = 1_800_000_000, Taxes = 180_000, ProtectedGuards = 15_000, MoonshineLimit = 2_900_000,
                RequiredFame = 150,
            },
            23 => new HouseData
            {
                Price = 2_200_000_000, Taxes = 220_000, ProtectedGuards = 17_000, MoonshineLimit = 3_300_000,
                RequiredFame = 200,
            },
            24 => new HouseData
            {
                Price = 2_600_000_000, Taxes = 260_000, ProtectedGuards = 19_500, MoonshineLimit = 3_700_000,
                RequiredFame = 250,
            },
            25 => new HouseData
            {
                Price = 3_000_000_000, Taxes = 300_000, ProtectedGuards = 21_500, MoonshineLimit = 4_000_000,
                RequiredFame = 320,
            },
            26 => new HouseData
            {
                Price = 3_500_000_000, Taxes = 350_000, ProtectedGuards = 23_500, MoonshineLimit = 4_400_000,
                RequiredFame = 400,
            },
            27 => new HouseData
            {
                Price = 4_000_000_000, Taxes = 400_000, ProtectedGuards = 25_000, MoonshineLimit = 5_000_000,
                RequiredFame = 500,
            },
            28 => new HouseData
            {
                Price = 4_500_000_000, Taxes = 450_000, ProtectedGuards = 27_500, MoonshineLimit = 5_400_000,
                RequiredFame = 800,
            },
            29 => new HouseData
            {
                Price = 5_500_000_000, Taxes = 550_000, ProtectedGuards = 30_000, MoonshineLimit = 5_800_000,
                RequiredFame = 1_100,
            },
            30 => new HouseData
            {
                Price = 6_500_000_000, Taxes = 650_000, ProtectedGuards = 32_500, MoonshineLimit = 6_200_000,
                RequiredFame = 1_400,
            },
            31 => new HouseData
            {
                Price = 7_500_000_000, Taxes = 750_000, ProtectedGuards = 35_000, MoonshineLimit = 6_600_000,
                RequiredFame = 1_700,
            },
            32 => new HouseData
            {
                Price = 9_000_000_000, Taxes = 900_000, ProtectedGuards = 37_500, MoonshineLimit = 7_000_000,
                RequiredFame = 2_000,
            },
            33 => new HouseData
            {
                Price = 10_100_000_000, Taxes = 1_100_000, ProtectedGuards = 40_000, MoonshineLimit = 7_600_000,
                RequiredFame = 2_300,
            },
            34 => new HouseData
            {
                Price = 10_500_000_000, Taxes = 1_500_000, ProtectedGuards = 42_500, MoonshineLimit = 8_400_000,
                RequiredFame = 2_600,
            },
            35 => new HouseData
            {
                Price = 20_000_000_000, Taxes = 2_000_000, ProtectedGuards = 45_000, MoonshineLimit = 9_400_000,
                RequiredFame = 2_900,
            },
            36 => new HouseData
            {
                Price = 30_000_000_000, Taxes = 3_000_000, ProtectedGuards = 47_500, MoonshineLimit = 10_600_000,
                RequiredFame = 3_200,
            },
            37 => new HouseData
            {
                Price = 40_000_000_000, Taxes = 4_000_000, ProtectedGuards = 50_000, MoonshineLimit = 12_000_000,
                RequiredFame = 3_500,
            },
            38 => new HouseData
            {
                Price = 50_000_000_000, Taxes = 5_000_000, ProtectedGuards = 52_500, MoonshineLimit = 13_600_000,
                RequiredFame = 3_800,
            },
            39 => new HouseData
            {
                Price = 70_000_000_000, Taxes = 7_000_000, ProtectedGuards = 55_000, MoonshineLimit = 15_400_000,
                RequiredFame = 4_100,
            },
            40 => new HouseData
            {
                Price = 100_000_000_000, Taxes = 10_000_000, ProtectedGuards = 60_000, MoonshineLimit = 17_400_000,
                RequiredFame = 4_500,
            },
            41 => new HouseData
            {
                Price = 150_000_000_000, Taxes = 15_000_000, ProtectedGuards = 65_000, MoonshineLimit = 19_600_000,
                RequiredFame = 4_900,
            },
            42 => new HouseData
            {
                Price = 200_000_000_000, Taxes = 20_000_000, ProtectedGuards = 70_000, MoonshineLimit = 22_000_000,
                RequiredFame = 5_300,
            },
            43 => new HouseData
            {
                Price = 250_000_000_000, Taxes = 25_000_000, ProtectedGuards = 75_000, MoonshineLimit = 24_600_000,
                RequiredFame = 5_700,
            },
            44 => new HouseData
            {
                Price = 350_000_000_000, Taxes = 35_000_000, ProtectedGuards = 80_000, MoonshineLimit = 27_400_000,
                RequiredFame = 6_100,
            },
            45 => new HouseData
            {
                Price = 450_000_000_000, Taxes = 45_000_000, ProtectedGuards = 85_000, MoonshineLimit = 30_400_000,
                RequiredFame = 6_500,
            },
            46 => new HouseData
            {
                Price = 600_000_000_000, Taxes = 60_000_000, ProtectedGuards = 90_000, MoonshineLimit = 33_600_000,
                RequiredFame = 6_900,
            },
            47 => new HouseData
            {
                Price = 800_000_000_000, Taxes = 80_000_000, ProtectedGuards = 95_000, MoonshineLimit = 37_000_000,
                RequiredFame = 7_300,
            },
            48 => new HouseData
            {
                Price = 1_000_000_000_000, Taxes = 100_000_000, ProtectedGuards = 100_000, MoonshineLimit = 40_600_000,
                RequiredFame = 7_700,
            },
            _ => throw new Exception("Invalid house level."),
        };
    }
}