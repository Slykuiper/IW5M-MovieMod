using System;
using System.Collections;
using Addon;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;

namespace SlyMovieMod
{
    public class Main : CPlugin
    {
        string str_noclipstatus = "disabled";
        string str_3rdstatus = "disabled";
        struct OldOrigins
        {
            public float flX, flY, flZ;
        };

        OldOrigins[] pSaves;
        float[] parsed;

        public override void OnServerLoad()
        {
            ServerPrint("|-------------------------------------------------------------|");
            ServerPrint("|==============MW3 Movie Making Mod by Slykuiper==============|");
            ServerPrint("|================|http://youtube.com/slykuiper|===============|");
            ServerPrint("|====================|http://slykuiper.com|===================|");
            ServerPrint("|-------------------------------------------------------------|");

            pSaves = new OldOrigins[18];
            parsed = new float[3];
        }

        public override ChatType OnSay(string Message, ServerClient Client, bool Teamchat)
        {
            if (Message == "!help")
            {
                TellClient(Client.ClientNum, "Main cmds: !help, !map, !ammo, !perks, !noclip, !3rd, !savepos, !loadpos", true);
                TellClient(Client.ClientNum, "Player cmds: !give, !playermodel, !move", true);
                return ChatType.ChatNone;
            }
            if (Message == "!noclip")
            {
                if (str_noclipstatus == "disabled"){str_noclipstatus = "enabled";Client.Other.NoClip = true;}
                else{str_noclipstatus = "disabled";Client.Other.NoClip = false;}
            }
            if (Message == "!3rd")
            {
                if (str_3rdstatus == "disabled"){str_3rdstatus = "enabled";SetClientDvar(Client.ClientNum, "cg_thirdperson \"1\"");}
                else{str_3rdstatus = "disabled";SetClientDvar(Client.ClientNum, "cg_thirdperson \"0\"");}
            }
            if (Message == "!perks")
            {
                Client.Other.SetPerk(GetPerk("specialty_longersprint"));
                Client.Other.SetPerk(GetPerk("specialty_fastmantle"));
                Client.Other.SetPerk(GetPerk("specialty_fastreload"));
                Client.Other.SetPerk(GetPerk("specialty_quickdraw"));
                Client.Other.SetPerk(GetPerk("specialty_fastoffhand"));
                Client.Other.SetPerk(GetPerk("specialty_assists"));
                Client.Other.SetPerk(GetPerk("specialty_bulletaccuracy"));
                Client.Other.SetPerk(GetPerk("specialty_steadyaimpro"));
                Client.Other.SetPerk(GetPerk("specialty_fastsprintrecovery"));
                Client.Other.SetPerk(GetPerk("specialty_quieter"));
                Client.Other.SetPerk(GetPerk("specialty_falldamage"));
                Client.Other.SetPerk(GetPerk("specialty_stalker"));
                Client.Other.SetPerk(GetPerk("specialty_autospot"));
                Client.Other.SetPerk(GetPerk("specialty_holdbreath"));

                TellClient(Client.ClientNum, "Given perks.", true);
                return ChatType.ChatNone;
            }
            if (Message == "!ammo")
            {
                int equipAmmo = 2;
                int gunAmmo = 10;
                int clipAmmo = 20;

                Client.Ammo.PrimaryAmmo = gunAmmo;
                Client.Ammo.SecondaryAmmo = gunAmmo;
                Client.Ammo.PrimaryAmmoClip = clipAmmo;
                Client.Ammo.SecondaryAmmoClip = clipAmmo;
                Client.Ammo.OffhandAmmo = equipAmmo;
                Client.Ammo.EquipmentAmmo = equipAmmo;
                return ChatType.ChatNone;
            }
            if (Message.ToLower().StartsWith("!give"))
            {
                string[] tokens = Message.Split(' ');
                if (tokens.Length != 4)
                    TellClient(Client.ClientNum, "Format: !give [playername] [weapon] [camo]", true);

                if (tokens.Length == 4)
                {
                    string playerName = tokens[1];
                    string weaponName = tokens[2];
                    string weaponCamo = tokens[3];
                    ServerClient player = null;
                    player = GetClientByName(playerName);
                    giveWeapon(player, weaponName, weaponCamo);
                }
                return ChatType.ChatNone;
            }
            if (Message.StartsWith("!savepos"))
            {
                pSaves[Client.ClientNum].flX = Client.OriginX;
                pSaves[Client.ClientNum].flY = Client.OriginY;
                pSaves[Client.ClientNum].flZ = Client.OriginZ;

                TellClient(Client.ClientNum, "Position saved.", true);

                return ChatType.ChatNone;
            }
            if (Message.StartsWith("!loadpos"))
            {
                Client.OriginX = pSaves[Client.ClientNum].flX;
                Client.OriginY = pSaves[Client.ClientNum].flY;
                Client.OriginZ = pSaves[Client.ClientNum].flZ;

                TellClient(Client.ClientNum, "Position Loaded.", true);

                return ChatType.ChatNone;
            }
            if (Message.StartsWith("!move"))
            {
                string[] tokens = Message.Split(' ');
                if (tokens.Length < 2)
                    TellClient(Client.ClientNum, "Format: !move [playername]", true);

                ServerClient player = null;
                player = GetClientByName(tokens[1]);

                player.OriginX = Client.OriginX;
                player.OriginY = Client.OriginY;
                player.OriginZ = Client.OriginZ;

                TellClient(Client.ClientNum, tokens[1] + " moved to you.", true);

                return ChatType.ChatNone;
            }

            if (Message.ToLower().StartsWith("!map"))
            {
                string[] tokens = Message.Split(' ');
                if (tokens.Length != 2)
                    TellClient(Client.ClientNum, "Format: !map [name]", true);

                if (tokens.Length == 2)
                {
                    string str_mapname = tokens[1];
                    ServerCommand("map " + mapChange(str_mapname));
                }
                return ChatType.ChatNone;
            }

            if (Message.ToLower().StartsWith("!playermodel"))
            {
                string[] tokens = Message.Split(' ');
                if (tokens.Length != 4)
                    TellClient(Client.ClientNum, "Format: !playermodel [playername] [bodyModel] [headModel]", true);

                if (tokens.Length == 4)
                {
                    string playerName = tokens[1];
                    string bodyModel = tokens[2];
                    string headModel = tokens[3];
                    ServerClient player = null;
                    player = GetClientByName(playerName);
                    player.Other.SetPlayerModel(bodyModel);
                    player.Other.SetPlayerHeadModel(headModel);
                }
                return ChatType.ChatNone;
            }
            else
            {
                return ChatType.ChatContinue;
            }
        }

        public override void OnPlayerConnect(ServerClient Client)
        {

        }

        public override void OnPlayerDisconnect(ServerClient Client)
        {

        }

        public override void OnPlayerSpawned(ServerClient Client)
        {

        }

        public override void OnPlayerChangeWeapon(ServerClient Client, int WeaponID)
        {

        }

        public override void OnPlayerUse(ServerClient Client)
        {

        }

        public string mapChange(string mapname)
        {
            switch (mapname)
            {
                // default maps
                case "lockdown": return "mp_alpha";
                case "bootleg": return "mp_bootleg";
                case "mission": return "mp_bravo";
                case "carbon": return "mp_carbon";
                case "dome": return "mp_dome";
                case "downturn": return "mp_exchange";
                case "hardhat": return "mp_hardhat";
                case "interchange": return "mp_interchange";
                case "fallen": return "mp_lambeth";
                case "bakaara": return "mp_mogadishu";
                case "resistance": return "mp_paris";
                case "arkaden": return "mp_plaza2";
                case "outpost": return "mp_radar";
                case "seatown": return "mp_seatown";
                case "underground": return "mp_underground";
                case "village": return "mp_village";
                // dlc aquired
                case "aground": return "mp_aground_ss";
                case "erosion": return "mp_courtyard_ss";
                case "terminal": return "mp_terminal_cls";
                case "lookout": return "mp_restrepo_ss";
                case "foundation": return "mp_cement";
                case "getaway": return "mp_hillside_ss";
                case "oasis": return "mp_qadeem";
                case "sanctuary": return "mp_meteora";
                // dlc not aquired
                case "liberation": return "mp_italy";
                case "piazza": return "mp_piazza";
                case "overwatch": return "mp_overwatch";
                case "blackbox": return "mp_plane";
                case "vortex": return "mp_six_ss";
                case "intersection": return "mp_crosswalk_ss";
                case "uturn": return "mp_burn_ss";
                case "decommission": return "mp_shipbreaker";
                case "offshore": return "mp_oilrig";
                case "boardwalk": return "mp_boardwalk";
                case "gulch": return "mp_moab";
                case "parish": return "mp_parish";
                default: return "empty";
            };
        }
        public void giveWeapon(ServerClient player, string weaponName, string weaponCamo)
        {
            int createdWeapon = GetWeapon(weaponName + "_camo" + camoChange(weaponCamo));
            player.Other.PrimaryWeapon = createdWeapon;
            player.Other.CurrentWeapon = createdWeapon;
        }
        public string camoChange(string weaponCamo)
        {
            switch (weaponCamo)
            {
                case "none": return "";
                case "0": return "";
                case "classic": return "01";
                case "snow": return "02";
                case "multi": return "03";
                case "d_urban": return "04";
                case "hex": return "05";
                case "choco": return "06";
                case "snake": return "07";
                case "blue": return "08";
                case "red": return "09";
                case "autumn": return "10";
                case "gold": return "11";
                case "marine": return "12";
                case "winter": return "13";
                default: return "";
            };
        }
        public ServerClient GetClientByName(string name)
        {
            if (name == null)
                return null;

            ServerClient client = null;
            for (int i = 0; i < 18; i++)
            {
                client = GetClient(i);

                if (client == null)
                    continue;

                if (client.Name.ToLower().Contains(name.ToLower()))
                    return client;
            }

            return null;
        }
    }
}
