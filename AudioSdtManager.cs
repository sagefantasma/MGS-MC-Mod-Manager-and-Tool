using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANTIBigBoss_MGS_Mod_Manager
{

    public class SongInfo
    {

        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }
    }

    public static class AudioSdtManager
    {
        public static List<SongInfo> MGS3Songs = new List<SongInfo>
        {
            new SongInfo { DisplayName = "Outdoor Alert/Evasion", FileName = "act_set2_2v2_1.sdt" },
            new SongInfo { DisplayName = "Outdoor Caution", FileName = "act_set2_2v2_2.sdt" },
            new SongInfo { DisplayName = "Outdoor/Indoor Alert/Evasion", FileName = "act_set3a_1.sdt" },
            new SongInfo { DisplayName = "Outdoor/Indoor Caution", FileName = "act_set3a_2.sdt" },
            new SongInfo { DisplayName = "Eva Mission Alert/Evasion", FileName = "act_set4a_1.sdt" },
            new SongInfo { DisplayName = "Eva Mission Caution", FileName = "act_set4a_2.sdt" },
            new SongInfo { DisplayName = "SvM Alert/Evasion", FileName = "monkey_gets.sdt" },
            new SongInfo { DisplayName = "Shagohod Boss Battle", FileName = "boss_chago_a.sdt" },
            new SongInfo { DisplayName = "boss_colonel_nointro", FileName = "boss_colonel_nointro.sdt" },
            new SongInfo { DisplayName = "Volgin Boss", FileName = "boss_coronel.sdt" },
            new SongInfo { DisplayName = "Fear Boss", FileName = "boss_fear.sdt" },
            new SongInfo { DisplayName = "Fury Boss", FileName = "boss_fury.sdt" },
            new SongInfo { DisplayName = "Ocelot Boss", FileName = "boss_ocelot_a.sdt" },
            new SongInfo { DisplayName = "Pain Boss", FileName = "boss_pain.sdt" },
            new SongInfo { DisplayName = "Sorrow Boss", FileName = "boss_sorrow.sdt" },
            new SongInfo { DisplayName = "The Boss", FileName = "boss_theboss.sdt" },
            new SongInfo { DisplayName = "SvM Related", FileName = "boss_theboss_monkey.sdt" },
            new SongInfo { DisplayName = "SvM - Final Mission", FileName = "boss_theboss_suv.sdt" },
            new SongInfo { DisplayName = "The End ambient", FileName = "boss_theend.sdt" },
            new SongInfo { DisplayName = "bsosfall01", FileName = "bsofall01.sdt" },
            new SongInfo { DisplayName = "canyon01", FileName = "canyon01.sdt" },
            new SongInfo { DisplayName = "cave01", FileName = "cave01.sdt" },
            new SongInfo { DisplayName = "cave02", FileName = "cave02.sdt" },
            new SongInfo { DisplayName = "deserted01", FileName = "deserted01.sdt" },
            new SongInfo { DisplayName = "drain01", FileName = "drain01.sdt" },
            new SongInfo { DisplayName = "Duel in Wig", FileName = "event_roulette.sdt" },
            new SongInfo { DisplayName = "Eva Bike Chase 1", FileName = "event_sidecar1.sdt" },
            new SongInfo { DisplayName = "Eva Bike Chase 2", FileName = "event_sidecar2.sdt" },
            new SongInfo { DisplayName = "Sniping the C3 Theme", FileName = "event_sniping.sdt" },
            new SongInfo { DisplayName = "factory05", FileName = "factory05.sdt" },
            new SongInfo { DisplayName = "fall_river01", FileName = "fall_river01.sdt" },
            new SongInfo { DisplayName = "fort01", FileName = "fort01.sdt" },
            new SongInfo { DisplayName = "Indoor Area ambient", FileName = "indoor01.sdt" },
            new SongInfo { DisplayName = "indoor01n", FileName = "indoor01n.sdt" },
            new SongInfo { DisplayName = "indoor101", FileName = "indoor101.sdt" },
            new SongInfo { DisplayName = "indoor12", FileName = "indoor12.sdt" },
            new SongInfo { DisplayName = "jungle01", FileName = "jungle01.sdt" },
            new SongInfo { DisplayName = "jungle02", FileName = "jungle02.sdt" },
            new SongInfo { DisplayName = "jungle05", FileName = "jungle05.sdt" },
            new SongInfo { DisplayName = "jungle11", FileName = "jungle11.sdt" },
            new SongInfo { DisplayName = "mang_base01", FileName = "mang_base01.sdt" },
            new SongInfo { DisplayName = "mangrove02", FileName = "mangrove02.sdt" },           
            new SongInfo { DisplayName = "mountain03", FileName = "mountain03.sdt" },
            new SongInfo { DisplayName = "mountain04", FileName = "mountain04.sdt" },
            new SongInfo { DisplayName = "night_canyon01", FileName = "night_canyon01.sdt" },
            new SongInfo { DisplayName = "night_cave02", FileName = "night_cave02.sdt" },
            new SongInfo { DisplayName = "night_deserted01", FileName = "night_deserted01.sdt" },
            new SongInfo { DisplayName = "night_mangrove02", FileName = "night_mangrove02.sdt" },
            new SongInfo { DisplayName = "night_pond01", FileName = "night_pond01.sdt" },
            new SongInfo { DisplayName = "night01", FileName = "night01.sdt" },
            new SongInfo { DisplayName = "night51", FileName = "night51.sdt" },
            new SongInfo { DisplayName = "pond01", FileName = "pond01.sdt" },
            new SongInfo { DisplayName = "pond02", FileName = "pond02.sdt" },
            new SongInfo { DisplayName = "CQC Intro Theme", FileName = "title_bgm.sdt" },
            new SongInfo { DisplayName = "Before Ladder", FileName = "tunnel03.sdt" },
            new SongInfo { DisplayName = "Snake Eater Ladder", FileName = "tunnel03_eater.sdt" },
            new SongInfo { DisplayName = "underground01", FileName = "underground01.sdt" },
            new SongInfo { DisplayName = "underground02", FileName = "underground02.sdt" },
            new SongInfo { DisplayName = "underground03", FileName = "underground03.sdt" }
        };

        public static List<SongInfo> MG1Songs = new List<SongInfo>
        {
            new SongInfo { DisplayName = "MG1 Pre-Game Over", FileName = "mg1_bgm00_playerout.sdt" },
            new SongInfo { DisplayName = "MG1 Game Over", FileName = "mg1_bgm01_continue.sdt" },
            new SongInfo { DisplayName = "Red Alert", FileName = "mg1_bgm02_kiken.sdt" },
            new SongInfo { DisplayName = "Theme of Tara", FileName = "mg1_bgm03_main.sdt" },
            new SongInfo { DisplayName = "Sneaking Mission", FileName = "mg1_bgm04_underground.sdt" },
            new SongInfo { DisplayName = "Mercenary", FileName = "mg1_bgm05_boss.sdt" },
            new SongInfo { DisplayName = "Operation Intrude N313", FileName = "mg1_bgm06_opening.sdt" },
            new SongInfo { DisplayName = "TX-55 Metal Gear", FileName = "mg1_bgm07_metalgear.sdt" },
            new SongInfo { DisplayName = "Escape Theme (Beyond Big Boss)", FileName = "mg1_bgm08_escape.sdt" },
            new SongInfo { DisplayName = "Return of the Foxhounder", FileName = "mg1_bgm09_ending.sdt" }
        };

        public static List<SongInfo> MG2Songs = new List<SongInfo>
        {
            new SongInfo { DisplayName = "MG2 Pre-Game Over", FileName = "mg2_bgm00_playerout.sdt" },
            new SongInfo { DisplayName = "MG2 Game Over", FileName = "mg2_bgm01_gameover.sdt" },
            new SongInfo { DisplayName = "Level 3 Warning (Alert)", FileName = "mg2_bgm02_level3.sdt" },
            new SongInfo { DisplayName = "Level 2 Warning (Alert)", FileName = "mg2_bgm03_level2.sdt" },
            new SongInfo { DisplayName = "Frequency 140.85 (Sneaking)", FileName = "mg2_bgm04_main01.sdt" },
            new SongInfo { DisplayName = "Advance Immediately (Sneaking)", FileName = "mg2_bgm05_main02.sdt" },
            new SongInfo { DisplayName = "Shallow (Sneaking)", FileName = "mg2_bgm06_main03.sdt" },
            new SongInfo { DisplayName = "Imminent (Sneaking)", FileName = "mg2_bgm07_main04.sdt" },
            new SongInfo { DisplayName = "Level 1 Warning (Sneaking)", FileName = "mg2_bgm08_main05.sdt" },
            new SongInfo { DisplayName = "The Front Line (Sneaking)", FileName = "mg2_bgm09_main06.sdt" },
            new SongInfo { DisplayName = "Infiltration (Sneaking)", FileName = "mg2_bgm10_main07.sdt" },
            new SongInfo { DisplayName = "Return to Dust (Sneaking)", FileName = "mg2_bgm11_main08.sdt" },
            new SongInfo { DisplayName = "Under the Cloud of Darkness (Sneaking)", FileName = "mg2_bgm12_main09.sdt" },
            new SongInfo { DisplayName = "Reprieve of the Doctor (Sneaking)", FileName = "mg2_bgm13_main10.sdt" },
            new SongInfo { DisplayName = "An Advance (Sneaking)", FileName = "mg2_bgm14_talk1.sdt" },
            new SongInfo { DisplayName = "Nightfall (Sneaking)", FileName = "mg2_bgm15_talk2.sdt" },
            new SongInfo { DisplayName = "Wavelet (Sneaking)", FileName = "mg2_bgm16_talk3.sdt" },
            new SongInfo { DisplayName = "Tears (Sneaking)", FileName = "mg2_bgm17_talk4.sdt" },
            new SongInfo { DisplayName = "Killers - Intro (Caution)", FileName = "mg2_bgm18_bosstalk1.sdt" },
            new SongInfo { DisplayName = "In Security (Caution)", FileName = "mg2_bgm19_bosstalk2.sdt" },
            new SongInfo { DisplayName = "Killers - Battle (Boss)", FileName = "mg2_bgm20_boss1.sdt" },
            new SongInfo { DisplayName = "Battle Against Time (Boss)", FileName = "mg2_bgm21_boss2.sdt" },
            new SongInfo { DisplayName = "Mechanic (Boss)", FileName = "mg2_bgm22_boss3.sdt" },
            new SongInfo { DisplayName = "Night Fright Boss Theme (Caution)", FileName = "mg2_bgm23_boss4.sdt" },
            new SongInfo { DisplayName = "The National Anthem of Zanziber Land Part 1 (Jingle)", FileName = "mg2_bgm24_zltheme1.sdt" },
            new SongInfo { DisplayName = "The National Anthem of Zanziber Land Part 2 (Jingle)", FileName = "mg2_bgm25_zltheme2.sdt" },
            new SongInfo { DisplayName = "Chasing The Green Beret (Caution)", FileName = "mg2_bgm26_chase.sdt" },
            new SongInfo { DisplayName = "Ambient Noises", FileName = "mg2_bgm27_exhaust.sdt" },
            new SongInfo { DisplayName = "A Notice - Area Start (Jingle)", FileName = "mg2_bgm28_startdemo.sdt" },
            new SongInfo { DisplayName = "First Instruction - Opening Radio", FileName = "mg2_bgm29_startmusen.sdt" },
            new SongInfo { DisplayName = "Gustava/Natasha's Death Theme", FileName = "mg2_bgm30_natashadeath.sdt" },
            new SongInfo { DisplayName = "Spiral Staircase Battle (Alert)", FileName = "mg2_bgm31_spiralstair.sdt" },
            new SongInfo { DisplayName = "Hang Glider (Jingle)", FileName = "mg2_bgm32_hangglider.sdt" },
            new SongInfo { DisplayName = "Big Boss (Boss)", FileName = "mg2_bgm33_finalbase.sdt" },
            new SongInfo { DisplayName = "Escape Theme (Alert)", FileName = "mg2_bgm34_escape.sdt" },
            new SongInfo { DisplayName = "Theme of Solid Snake - Intro Theme (Sneaking)", FileName = "mg2_bgm35_opening1.sdt" },
            new SongInfo { DisplayName = "Zanzibar Breeze (Sneaking)", FileName = "mg2_bgm36_opening2.sdt" },
            new SongInfo { DisplayName = "Return - Ending Theme 1", FileName = "mg2_bgm37_ending1.sdt" },
            new SongInfo { DisplayName = "Red Sun - Ending Theme 2", FileName = "mg2_bgm38_ending2.sdt" },
            new SongInfo { DisplayName = "Farewell - Ending Theme 3", FileName = "mg2_bgm39_ending3.sdt" },
            new SongInfo { DisplayName = "Swing Swing 'B' Jam Blues (Secret Radio BGM 1)", FileName = "mg2_bgm40_secret1.sdt" },
            new SongInfo { DisplayName = "Swing Swing 'A' Jam Blues (Secret Radio BGM 2)", FileName = "mg2_bgm41_secret2.sdt" },
            new SongInfo { DisplayName = "Boss Intro (Jingle)", FileName = "mg2_intro00_bosscontact1.sdt" },
            new SongInfo { DisplayName = "Gustava/Natasha's Death Intro (Jingle)", FileName = "mg2_intro08_natasha1.sdt" },
            new SongInfo { DisplayName = "Metal Gear Intro (Jingle)", FileName = "mg2_intro10_metalgear.sdt" },
            new SongInfo { DisplayName = "Helicopter Noises", FileName = "mg2_se64_propelle.sdt" }
        };
    }
}