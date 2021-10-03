using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;


namespace HightMapToTimberborn
{

    class Program
    {
        //Map file name.
        static String mapName;

        //Random.
        static Random rnd = new Random(); 

        //Map names.
        static String hightMapNam = @"HightMap.png";
        static String objectMapNam = @"ObjectMap.png";
        static String waterMapNam = @"WaterMap.png";
        static String irrigationMapNam = @"irrigationMap.png";

        //Output file info, default size.
        static String gameVersion = "ND-02-10-2021-rev1";
        static String time = "";
        static int widht = 16, hight = 16;

        //ID creating varibels
        static String constID = "ffffffff-ffff-ffff-ffff-fffffff";
        static int changebleID = 0;

        //Maps.
        static String hightMap;
        static String waterMap;
        static String irrigationMap;
        static String flows;
        static void Main()
        {            
            time = DateTimeOffset.Now.ToString();
            Console.WriteLine("Write map name");
            mapName = Console.ReadLine();

            hightMap = GetHightInfo();
            Console.WriteLine("--Hight map converted--");
            waterMap = GetWaterMap();
            Console.WriteLine("--Water map converted--");
            irrigationMap = GetIrrigationMap();
            Console.WriteLine("--Irrigation map converted--");
            flows = GetFlows();
            Console.WriteLine("--Flows generated--");            

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");

            GenerateJson(waterMap, hightMap, irrigationMap, flows, GetObjects());
            //Console.WriteLine(GenerateJson(waterMap, hightMap, irrigationMap, flows, GetObjects()));
        }
        private static int Map(int value, int fromLow, int fromHigh, int toLow, int toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }
        //Map processing------------------------------------------------------------------------------------------------------
        private static String GetHightInfo()
        {
            Rgba32 color;
            int grayColor;            
            String res = "";
            try
            {
                using (var image = Image.Load<Rgba32>(hightMapNam))
                {
                    widht = image.Width;
                    hight = image.Height;
                    for (int i = 0; i < image.Width; i++)
                    {
                        for (int j = 0; j < image.Height; j++)
                        {
                            color = image[i, j];
                            grayColor = Map((color.R + color.G + color.B) / 3, 0, 255, 0, 16);
                            res = res + " " + grayColor;
                        }
                    }
                    res.Remove(1);
                    return res.TrimStart(' ');
                }
            }
            catch
            {
                Console.WriteLine("WARNING missing HightMap.png");
                for (int i = 0; i < (hight * widht); i++)
                {
                    res += " 0";                    
                }                
                return res.TrimStart(' ');
            }

        }
        private static int GetPixelHight(int i, int j)
        {
            Rgba32 color;
            int grayColor;
            int res;
            try
            {
                using (var image = Image.Load<Rgba32>(hightMapNam))
                {
                    color = image[i, j];
                    grayColor = Map((color.R + color.G + color.B) / 3, 0, 255, 0, 16);
                    res = grayColor;
                    return res;
                }
            }
            catch
            {                              
                return 0;
            }
        }
        private static String GetWaterMap()
        {
            Rgba32 color;
            float alphaColor;
            String res = "";
            try
            {
                using (var image = Image.Load<Rgba32>(waterMapNam))
                {
                    for (int i = 0; i < image.Width; i++)
                    {
                        for (int j = 0; j < image.Height; j++)
                        {
                            color = image[i, j];
                            alphaColor = Map((color.A), 0, 255, 0, 32);
                            res = res + " " + alphaColor/2;
                        }
                    }
                    res.Remove(1);
                    return res.TrimStart(' ').Replace(',','.');
                }
            }
            catch
            {
                Console.WriteLine("WARNING missing WaterMap.png");
                for (int i = 0; i < (hight * widht); i++)
                {
                    res += " 0";
                }
                return res.TrimStart(' ').Replace(',', '.');
            }
        }
        private static String GetIrrigationMap()
        {
            Rgba32 color;
            float alphaColor;
            String res = "";
            try
            {
                using (var image = Image.Load<Rgba32>(irrigationMapNam))
                {
                    for (int i = 0; i < image.Width; i++)
                    {
                        for (int j = 0; j < image.Height; j++)
                        {
                            color = image[i, j];
                            alphaColor = Map((color.A), 0, 255, 0, 32);
                            res = res + " " + alphaColor / 2;
                        }
                    }
                    res.Remove(1);
                    return res.TrimStart(' ').Replace(',', '.');
                }
            }
            catch
            {
                Console.WriteLine("WARNING missing irrigationMap.png");
                for (int i = 0; i < (hight * widht); i++)
                {
                    res += " 0";
                }
                return res.TrimStart(' ').Replace(',', '.');
            }
        }
        private static String GetFlows()
        {
            String res = "";
            for (int i = 0; i < (hight * widht); i++)
            {
                res += " 0:0:0:0";
            }
            return res.TrimStart(' ').Replace(',', '.');
        }

        //ID generator--------------------------------------------------------------------------------------------------------
        private static String GenerateID()
        {            
            string id;
            id = "" + changebleID;
            changebleID++;
            while (id.Length < 5)
                {
                    id += "f";
                }
            Console.WriteLine(constID + id);
            return constID + id;
        }
        //Objects string generators-------------------------------------------------------------------------------------------
        private static String AddStartingPoint(int x, int y, int h)
        {
            String id;
            id = GenerateID();            
            return "{\"Id\":\"" + id + "\",\"Template\":\"StartingLocation\",\"Components\":{\"BlockObject\":{\"Coordinates\":{\"X\":" + x + ",\"Y\":" + y + ",\"Z\":" + h + "},\"Orientation\":{\"Value\":\"Cw180\"}}}}";
        }
        private static String AddWaterSource(int x, int y, int h, int strenght)
        {
            String id;
            id = GenerateID();
            return "{\"Id\":\"" + id + "\",\"Template\":\"WaterSource\",\"Components\":{\"WaterSource\":{\"SpecifiedStrength\":" + strenght + ",\"CurrentStrength\":" + strenght + "},\"BlockObject\":{\"Coordinates\":{\"X\":" + x + ",\"Y\":" + y +",\"Z\":" + h + "}}}}";
        }
        private static String AddBarier(int x, int y,int h)
        {
            String id;
            id = GenerateID();
            return "{\"Id\":\"" + id + "\",\"Template\":\"Barrier\",\"Components\":{\"BlockObject\":{\"Coordinates\":{\"X\":" + x + ",\"Y\":" + y + ",\"Z\":" + h + "}}}}";
        }
        private static String AddSlope(int x, int y, int h, int rot)
        {
            String id;
            id = GenerateID();
            return "{\"Id\":\"" + id + "\",\"Template\":\"Slope\",\"Components\":{\"ConstructionSite\":{\"BuildTimeProgressInHoursKey\":1.0},\"BlockObject\":{\"Coordinates\":{\"X\":" + x + ",\"Y\":" + y + ",\"Z\":" + h +"},\"Orientation\":{\"Value\":\"Cw" + rot + "\"}},\"Constructible\":{\"Finished\":true}}}";
        }
        private static String AddBlueberryBush(int x, int y, int h)
        {
            String id;
            id = GenerateID();
            return "{\"Id\":\"" + id + "\",\"Template\":\"BlueberryBush\",\"Components\":{\"BlockObject\":{\"Coordinates\":{\"X\":" + x + ",\"Y\":" + y + ",\"Z\":" + h + "}},\"Growable\":{\"GrowthProgress\":1.0},\"CoordinatesOffseter\":{\"CoordinatesOffset\":{\"X\":-0.0754646361,\"Y\":-0.242875636}},\"NaturalResourceModelRandomizer\":{\"Rotation\":" + GenerateNature() + ",\"DiameterScale\":" + GenerateNature(0.5f) + ",\"HeightScale\":" + GenerateNature(0.5f) + "},\"Yielder:Gatherable\":{\"Yield\":{\"Good\":{\"Id\":\"Berries\"},\"Amount\":0}},\"GatherableYieldGrower\":{\"GrowthProgress\":0.0},\"DryObject\":{\"IsDry\":true},\"LivingNaturalResource\":{\"IsDead\":true}}}";
        }
        private static String AddTree(int x, int y, int h, String treeTemplate, int scale, int logAmount)
        {
            String id;
            id = GenerateID();
            return "{\"Id\":\"" + id +"\",\"Template\":\"" + treeTemplate + "\",\"Components\":{\"BlockObject\":{\"Coordinates\":{\"X\":" + x + ",\"Y\":" + y + ",\"Z\":" + h + "}},\"Growable\":{\"GrowthProgress\":1.0},\"CoordinatesOffseter\":{\"CoordinatesOffset\":{\"X\":0,\"Y\":0}},\"NaturalResourceModelRandomizer\":{\"Rotation\":" + GenerateNature() + ",\"DiameterScale\":" + GenerateNature(scale) + ",\"HeightScale\":" + GenerateNature(scale) + "},\"Yielder:Cuttable\":{\"Yield\":{\"Good\":{\"Id\":\"Log\"},\"Amount\":" + logAmount + "}},\"DryObject\":{\"IsDry\":true},\"LivingNaturalResource\":{\"IsDead\":true}}}";
        }
        private static String AddRuin(int x, int y, int h, int lvl, int metalAmount, char ruinChar)
        {
            String id;
            id = GenerateID();
            return "{\"Id\":\"" + id + "\",\"Template\":\"RuinColumnH" + lvl + "\",\"Components\":{\"BlockObject\":{\"Coordinates\":{\"X\":" + x + ",\"Y\":" + y + ",\"Z\":" + h + "}},\"Yielder:Ruin\":{\"Yield\":{\"Good\":{\"Id\":\"ScrapMetal\"},\"Amount\":" + metalAmount + "}},\"RuinModels\":{\"VariantId\":\"" + ruinChar + "\"}}}";
        }
        //Generating objects list.--------------------------------------------------------------------------------------------
        private static String GetObjects()
        {
            Rgba32 color;
            String res = "";
            int cordHight;            
            try
            {                
                using (var image = Image.Load<Rgba32>(objectMapNam))
                {
                    for (int i = 0; i < image.Width; i++)
                    {
                        for (int j = 0; j < image.Height; j++)
                        {
                            color = image[i, j];
                            cordHight = GetPixelHight(i, j);
                            if (color.ToHex().Remove(6) == "99E550") //StartingPoint.
                            {                                
                                res = res + "," + AddStartingPoint(j+1, i+1, cordHight);
                                Console.WriteLine("Starting point generated at: " + j + " " + i + " " + cordHight);
                            }
                            if (color.ToHex().Remove(6) == "5B6EE1") //WaterSource.
                            {
                                res = res + "," + AddWaterSource(j, i, cordHight, (color.A)/32);
                                Console.WriteLine("Water Source generated at: " + j + " " + i + " " + cordHight + " .Stream strenght: " + (color.A/32));
                            }
                            if (color.ToHex().Remove(6) == "524B24") //Barier.
                            {
                                res = res + "," + AddBarier(j, i, cordHight);
                            }
                            if (color.ToHex().Remove(6) == "45283C") //Slope.
                            {
                                if (GetPixelHight(i, j - 1) > cordHight && GetPixelHight(i, j + 1) <= cordHight)
                               {
                                    res = res + "," + AddSlope(j, i, cordHight, 90);
                                }
                                else if (GetPixelHight(i + 1, j) > cordHight && GetPixelHight(i - 1, j) <= cordHight)
                                {
                                    res = res + "," + AddSlope(j, i, cordHight, 180);
                                }
                                else if (GetPixelHight(i, j + 1) > cordHight && GetPixelHight(i, j - 1) <= cordHight)
                                {
                                    res = res + "," + AddSlope(j, i, cordHight, 270);
                                }
                                else if (GetPixelHight(i - 1, j) > cordHight && GetPixelHight(i + 1, j) <= cordHight)
                                {
                                    res = res + "," + AddSlope(j, i, cordHight, 0);
                                }
                                else
                                {
                                    res = res + "," + AddSlope(j, i, cordHight, 90);
                                }

                            }
                            if (color.ToHex().Remove(6) == "76428A") //BlueberryBush.
                            {
                                res = res + "," + AddBlueberryBush(j, i, cordHight);
                            }
                            if (color.ToHex().Remove(6) == "4B692F") //Birch.
                            {
                                res = res + "," + AddTree(j, i, cordHight, "Birch", 1, 1);
                            }
                            if (color.ToHex().Remove(6) == "323C39") //Pine.
                            {
                                res = res + "," + AddTree(j, i, cordHight, "Pine", 1, 2);
                            }
                            if (color.ToHex().Remove(6) == "37946E") //Maple.
                            {
                                res = res + "," + AddTree(j, i, cordHight, "Maple", 1, 8);
                            }
                            if (color.ToHex().Remove(6) == "658D3F") //Big birch.
                            {
                                res = res + "," + AddTree(j, i, cordHight, "Birch", 2, 2);
                            }
                            if (color.ToHex().Remove(6) == "5D6F6A") //Big pine.
                            {
                                res = res + "," + AddTree(j, i, cordHight, "Pine", 2, 4);
                            }
                            if (color.ToHex().Remove(6) == "45B586") //Big maple.
                            {
                                res = res + "," + AddTree(j, i, cordHight, "Maple", 2, 16);
                            }
                            if (color.ToHex().Remove(6) == "DF7126") //Ruins.
                            {
                                res = res + "," + AddRuin(j, i, cordHight, (color.A/32), 15 * (color.A/32), GenerateRuin());
                            }
                        }
                    }
                    return res.TrimStart(',');
                }
            }
            catch
            {
                return "";
            }            
        }
        //Generating map file.------------------------------------------------------------------------------------------------
        private static void GenerateJson(String waterMap, String hightMap, String irrigationMap, String flows, String objects)
        {
            string finalString = "{\"GameVersion\":\"" + gameVersion + "\",\"Timestamp\":\"" + time + "\",\"Singletons\":{\"MapSize\":{\"Size\":{\"X\":" + widht + ",\"Y\":" + hight + "}},\"TerrainMap\":{\"Heights\":{\"Array\":\"" + hightMap + "\"}},\"WaterMap\":{\"WaterDepths\":{\"Array\":\"" + waterMap + "\"},\"Outflows\":{\"Array\":\"" + flows + "\"}},\"SoilMoistureSimulator\":{\"MoistureLevels\":{\"Array\":\"" + irrigationMap + "\"}}},\"Entities\":[" + objects + "]}";
            try 
            {
                //System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + mapName + ".json");
                //System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + mapName + ".json", "wdadawadaa");
                TextWriter file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + mapName + ".json");
                file.Write(finalString);
                file.Close();
                Console.WriteLine("Map file created successfully");
                Console.WriteLine("Please press any key to exit");
                Console.ReadKey();
            }
            catch
            {
                Console.WriteLine("Couldn't create file, directory acess denined or incorrect file name");
            }
            
        }

        //Random sutff.-------------------------------------------------------------------------------------------------------
        private static String GenerateNature(float scale)
        {
            return (scale + rnd.NextDouble()/2).ToString().Replace(',', '.');
        }
        private static String GenerateNature()
        {
            return rnd.Next(0, 360).ToString();
        }
        private static Char GenerateRuin()
        {            
            switch (rnd.Next(0, 4))
            {
                case 0:
                    return 'A';                    
                case 1:
                    return 'B';
                case 2:
                    return 'C';
                case 3:
                    return 'D';
                case 4:
                    return 'E';
                default:
                    return 'A';
            }
                        
        }
    }
}
