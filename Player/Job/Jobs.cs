namespace Player.Job;

public static class Jobs
{
    public static JobData GetJobData(long jobLevel)
    {
        return jobLevel switch
        {
            0 => new JobData
            {
                JobTitle = "Töötu",
                Income = 0,
                RequiredEducation = 0,
                BankLimit = 30_000,
                AllowedToBorrowPercentage = 0,
                MaxSafeLvl = 0,
            },
            1 => new JobData
            {
                JobTitle = "Autopesija",
                Income = 30_000,
                RequiredEducation = 20,
                BankLimit = 35_000,
                AllowedToBorrowPercentage = 0,
                MaxSafeLvl = 0,
            },
            2 => new JobData
            {
                JobTitle = "Koristaja",
                Income = 60_000,
                RequiredEducation = 1_200,
                BankLimit = 80_000,
                AllowedToBorrowPercentage = 0,
                MaxSafeLvl = 0,
            },
            3 => new JobData
            {
                JobTitle = "Laotööline",
                Income = 120_000,
                RequiredEducation = 3_400,
                BankLimit = 289_000,
                AllowedToBorrowPercentage = 0,
                MaxSafeLvl = 0,
            },
            4 => new JobData
            {
                JobTitle = "Turvamees",
                Income = 250_000,
                RequiredEducation = 4_700,
                BankLimit = 580_000,
                AllowedToBorrowPercentage = 10,
                MaxSafeLvl = 0,
            },
            5 => new JobData
            {
                JobTitle = "Torumees",
                Income = 500_000,
                RequiredEducation = 6_000,
                BankLimit = 620_000,
                AllowedToBorrowPercentage = 11,
                MaxSafeLvl = 0,
            },
            6 => new JobData
            {
                JobTitle = "Poemüüja",
                Income = 810_000,
                RequiredEducation = 7_800,
                BankLimit = 890_000,
                AllowedToBorrowPercentage = 12,
                MaxSafeLvl = 0,
            },
            7 => new JobData
            {
                JobTitle = "Õpetaja",
                Income = 1_300_000,
                RequiredEducation = 9_900,
                BankLimit = 1_470_000,
                AllowedToBorrowPercentage = 20,
                MaxSafeLvl = 0,
            },
            8 => new JobData
            {
                JobTitle = "Politseinik",
                Income = 2_200_000,
                RequiredEducation = 11_000,
                BankLimit = 1_900_000,
                AllowedToBorrowPercentage = 21,
                MaxSafeLvl = 0,
            },
            9 => new JobData
            {
                JobTitle = "Kokk",
                Income = 3_400_000,
                RequiredEducation = 13_500,
                BankLimit = 2_800_000,
                AllowedToBorrowPercentage = 22,
                MaxSafeLvl = 0,
            },
            10 => new JobData
            {
                JobTitle = "Ehitaja",
                Income = 5_000_000,
                RequiredEducation = 15_000,
                BankLimit = 4_000_000,
                AllowedToBorrowPercentage = 24,
                MaxSafeLvl = 0,
            },
            11 => new JobData
            {
                JobTitle = "Raamatupidaja",
                Income = 8_300_000,
                RequiredEducation = 18_500,
                BankLimit = 6_569_000,
                AllowedToBorrowPercentage = 26,
                MaxSafeLvl = 0,
            },
            12 => new JobData
            {
                JobTitle = "Peakokk",
                Income = 9_600_000,
                RequiredEducation = 22_800,
                BankLimit = 7_500_000,
                AllowedToBorrowPercentage = 28,
                MaxSafeLvl = 0,
            },
            13 => new JobData
            {
                JobTitle = "Maakler",
                Income = 12_000_000,
                RequiredEducation = 27_000,
                BankLimit = 15_000_000,
                AllowedToBorrowPercentage = 30,
                MaxSafeLvl = 0,
            },
            14 => new JobData
            {
                JobTitle = "Advokaat",
                Income = 15_000_000,
                RequiredEducation = 36_000,
                BankLimit = 30_000_000,
                AllowedToBorrowPercentage = 32,
                MaxSafeLvl = 0,
            },
            15 => new JobData
            {
                JobTitle = "Arst",
                Income = 19_000_000,
                RequiredEducation = 45_000,
                BankLimit = 50_000_000,
                AllowedToBorrowPercentage = 34,
                MaxSafeLvl = 0,
            },
            16 => new JobData
            {
                JobTitle = "Ärimees",
                Income = 24_000_000,
                RequiredEducation = 70_000,
                BankLimit = 125_000_000,
                AllowedToBorrowPercentage = 36,
                MaxSafeLvl = 1,
            },
            17 => new JobData
            {
                JobTitle = "Aktsionär",
                Income = 32_000_000,
                RequiredEducation = 105_000,
                BankLimit = 300_000_000,
                AllowedToBorrowPercentage = 38,
                MaxSafeLvl = 2,
            },
            18 => new JobData
            {
                JobTitle = "Firma juht",
                Income = 41_000_000,
                RequiredEducation = 150_000,
                BankLimit = 450_000_000,
                AllowedToBorrowPercentage = 40,
                MaxSafeLvl = 3,
            },
            19 => new JobData
            {
                JobTitle = "Panga direktor",
                Income = 55_000_000,
                RequiredEducation = 190_000,
                BankLimit = 870_000_000,
                AllowedToBorrowPercentage = 42,
                MaxSafeLvl = 4,
            },
            20 => new JobData
            {
                JobTitle = "Riigikogulane",
                Income = 68_000_000,
                RequiredEducation = 270_000,
                BankLimit = 1_400_000_000,
                AllowedToBorrowPercentage = 44,
                MaxSafeLvl = 5,
            },
            21 => new JobData
            {
                JobTitle = "Peaminister",
                Income = 90_000_000,
                RequiredEducation = 350_000,
                BankLimit = 2_000_000_000,
                AllowedToBorrowPercentage = 46,
                MaxSafeLvl = 6,
            },
            22 => new JobData
            {
                JobTitle = "President",
                Income = 130_000_000,
                RequiredEducation = 430_000,
                BankLimit = 4_000_000_000,
                AllowedToBorrowPercentage = 48,
                MaxSafeLvl = 7,
            },
            23 => new JobData
            {
                JobTitle = "Filmistaar",
                Income = 170_000_000,
                RequiredEducation = 510_000,
                BankLimit = 6_000_000_000,
                AllowedToBorrowPercentage = 50,
                MaxSafeLvl = 8,
            },
            _ => throw new Exception("Invalid job level."),
        };
    }

    public static SafeData GetSafeData(long jobLevel)
    {
        return jobLevel switch
        {
            0 => new SafeData
            {
                Price = 0,
                Capacity = 0,
                Taxes = 0,
                MinimumJobLevel = 0,
            },
            1 => new SafeData
            {
                Price = 500_000_000,
                Capacity = 1_000_000_000,
                Taxes = 90_000,
                MinimumJobLevel = 16,
            },
            2 => new SafeData
            {
                Price = 1_200_000_000,
                Capacity = 2_000_000_000,
                Taxes = 150_000,
                MinimumJobLevel = 17,
            },
            3 => new SafeData
            {
                Price = 1_500_000_000,
                Capacity = 3_000_000_000,
                Taxes = 300_000,
                MinimumJobLevel = 18,
            },
            4 => new SafeData
            {
                Price = 2_000_000_000,
                Capacity = 4_000_000_000,
                Taxes = 450_000,
                MinimumJobLevel = 19,
            },
            5 => new SafeData
            {
                Price = 2_600_000_000,
                Capacity = 5_600_000_000,
                Taxes = 600_000,
                MinimumJobLevel = 20,
            },
            6 => new SafeData
            {
                Price = 4_200_000_000,
                Capacity = 8_000_000_000,
                Taxes = 800_000,
                MinimumJobLevel = 21,
            },
            7 => new SafeData
            {
                Price = 7_500_000_000,
                Capacity = 12_000_000_000,
                Taxes = 1_000_000,
                MinimumJobLevel = 22,
            },
            8 => new SafeData
            {
                Price = 10_000_000_000,
                Capacity = 17_000_000_000,
                Taxes = 1_250_000,
                MinimumJobLevel = 23,
            },
            _ => throw new Exception("Invalid job level."),
        };
    }

    public static long GetExperiencedIncome(long jobLevel, long jobExp)
    {
        var job = GetJobData(jobLevel);
        var income = job.Income;
        var boostPercentage = GetBoostPercentage(jobExp);
        return income * (boostPercentage + 100) / 100;
    }

    private static long GetBoostPercentage(long jobExp)
    {
        return jobExp switch
        {
            > 96 * 16 => 25,
            > 96 * 8 => 20,
            > 96 * 4 => 15,
            > 96 * 2 => 10,
            > 96 * 1 => 5,
            _ => 0
        };
    }
}