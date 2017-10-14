namespace ItemImporter
{
    using System;
    using System.ComponentModel;
    using System.Data.Odbc;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmMain : Form
    {
        private Button btnBrowse;
        private Button btnConnect;
        private Button btnImport;
        private ComboBox cboEncryption;
        private ComboBox cboImport;
        private IContainer components;
        private OdbcConnection dbConnection;
        private byte encryption;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label lblDSN;
        private Label lblPWD;
        private Label lblSuffix;
        private Label lblUID;
        private ToolStripStatusLabel statusLabel;
        private StatusStrip statusStrip1;
        private TextBox txtDSN;
        private TextBox txtPath;
        private TextBox txtPWD;
        private TextBox txtSuffix;
        private TextBox txtUID;

        public frmMain()
        {
            this.InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog {
                Description = "Please select the Knight Online Data folder."
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.txtPath.Text = dialog.SelectedPath;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.dbConnection = new OdbcConnection($"DSN={this.txtDSN.Text}; UID={this.txtUID.Text}; PWD={this.txtPWD.Text};");
            try
            {
                this.dbConnection.Open();
                this.btnConnect.Enabled = false;
            }
            catch (OdbcException exception)
            {
                this.dbConnection.Dispose();
                this.dbConnection = null;
                MessageBox.Show(exception.Errors[0].Message, "A database error occurred");
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (this.txtPath.Text.Length == 0)
            {
                MessageBox.Show("Please select the Data folder first!");
            }
            else
            {
                if (this.dbConnection == null)
                {
                    this.btnConnect_Click(null, null);
                }
                this.encryption = 0;
                string text = this.cboEncryption.Text;
                if (text != null)
                {
                    if (text == "Standard")
                    {
                        this.encryption = 1;
                    }
                    else if (text == "New KOKO")
                    {
                        this.encryption = 2;
                    }
                }
                if (MessageBox.Show("Importing the client items to your database will WIPE ALL EXISTING ROWS. Are you sure you wish to proceed?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.btnImport.Enabled = false;
                    this.ImportProcess();
                    this.btnImport.Enabled = true;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.dbConnection != null)
            {
                this.dbConnection.Close();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

        private void ImportProcess()
        {
            TBL ItemOrgTbl;
            try
            {
                ItemOrgTbl = new TBL(this.txtPath.Text + @"\Item_org" + this.txtSuffix.Text + ".tbl", this.encryption);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Cannot read " + this.txtPath.Text + @"\Item_org" + this.txtSuffix.Text + ".tbl\n" + exception.Message + "\n" + exception.StackTrace);
                return;
            }
            int ExtTbls = (this.cboImport.Text == "Ext 0 -> 44") ? 0x2c : 0x17;
            TBL[] tblArray = new TBL[ExtTbls + 1];
            int index = 0;
            try
            {
                index = 0;
                while (index <= ExtTbls)
                {
                    tblArray[index] = new TBL(string.Concat(new object[] { this.txtPath.Text, @"\Item_ext_", index, this.txtSuffix.Text, ".tbl" }), this.encryption);
                    index++;
                }
            }
            catch (Exception exception2)
            {
                MessageBox.Show(string.Concat(new object[] { "Cannot read ", this.txtPath.Text, @"\Item_ext_", index, this.txtSuffix.Text, ".tbl\n", exception2.Message, "\n", exception2.StackTrace }));
                return;
            }
            try
            {
                this.Query("DROP TABLE ITEM");
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot drop ITEM table! Please make sure that " + this.txtUID.Text + " has permission to DROP tables in the selected database. Also make sure that the selected database contains the ITEM table.", "Error");
                return;
            }
            try
            {
                this.Query("CREATE TABLE [dbo].[ITEM]([Num] [int] NULL, [Extension] [tinyint] NULL, [strName] [varchar](50) NULL, [Description] [varchar](512) NULL,[isUnique] [int] NULL, \t[IconID] [int] NULL,\t[Kind] [tinyint] NULL, \t[Slot] [tinyint] NULL,\t[Race] [tinyint] NULL, \t[Class] [tinyint] NULL, \t[Damage] [smallint] NULL,\t[Delay] [smallint] NULL, \t[Range] [smallint] NULL,\t[Weight] [smallint] NULL, \t[Duration] [smallint] NULL,\t[BuyPrice] [int] NULL, \t[SellPrice] [int] NULL,\t[Ac] [smallint] NULL,\t[Countable] [tinyint] NULL,\t[Effect1] [int] NULL,\t[Effect2] [int] NULL,\t[ReqLevel] [tinyint] NULL,\t[ReqLevelMax] [tinyint] NULL,\t[ReqRank] [tinyint] NULL,\t[ReqTitle] [tinyint] NULL,\t[ReqStr] [tinyint] NULL,\t[ReqSta] [tinyint] NULL,\t[ReqDex] [tinyint] NULL,\t[ReqIntel] [tinyint] NULL,\t[ReqCha] [tinyint] NULL,\t[SellingGroup] [tinyint] NULL,\t[ItemType] [tinyint] NULL,\t[Hitrate] [smallint] NULL,\t[Evasionrate] [smallint] NULL,\t[DaggerAc] [smallint] NULL,\t[JamadarAc] [smallint] NULL,\t[SwordAc] [smallint] NULL,\t[MaceAc] [smallint] NULL,\t[AxeAc] [smallint] NULL,\t[SpearAc] [smallint] NULL,\t[BowAc] [smallint] NULL,\t[FireDamage] [tinyint] NULL,\t[IceDamage] [tinyint] NULL,\t[LightningDamage] [tinyint] NULL,\t[PoisonDamage] [tinyint] NULL,\t[HPDrain] [tinyint] NULL,\t[MPDamage] [tinyint] NULL,\t[MPDrain] [tinyint] NULL,\t[MirrorDamage] [tinyint] NULL,\t[Droprate] [tinyint] NULL,\t[StrB] [smallint] NULL,\t[StaB] [smallint] NULL,\t[DexB] [smallint] NULL,\t[IntelB] [smallint] NULL,\t[ChaB] [smallint] NULL,\t[MaxHpB] [smallint] NULL,\t[MaxMpB] [smallint] NULL,\t[FireR] [smallint] NULL,\t[ColdR] [smallint] NULL,\t[LightningR] [smallint] NULL,\t[MagicR] [smallint] NULL,\t[PoisonR] [smallint] NULL,\t[CurseR] [smallint] NULL,\t[ItemClass] [smallint] NULL,\t[ItemExt] [tinyint] NULL,\t[UpgradeNotice] [int] NULL,\t[NPbuyPrice] [int] NULL,\t[Bound] [tinyint] NULL)");
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot create ITEM table! Please make sure that " + this.txtUID.Text + " has permission to CREATE tables in the selected database.", "Error");
                return;
            }
            this.statusLabel.Text = "Importing items...";
            Application.DoEvents();
            int num3 = 0;
            for (int i = 0; i < ItemOrgTbl.getRows(); i++)
            {
                uint ItemNum = (uint)ItemOrgTbl.getValue(i, 0);
                byte Extension = (byte)ItemOrgTbl.getValue(i, 1);

                if (Extension <= ExtTbls)
                {
                    byte ItemExt = (byte)ItemOrgTbl.getValue(i, 1);
                    string strName = (string)ItemOrgTbl.getValue(i, 2);
                    string Description = (string)ItemOrgTbl.getValue(i, 3);
                    uint isUnique = (uint)ItemOrgTbl.getValue(i, 4);
                    uint dxtID = (uint)ItemOrgTbl.getValue(i, 7);
                    byte Kind = (byte)ItemOrgTbl.getValue(i, 10);
                    byte Slot = (byte)ItemOrgTbl.getValue(i, 12);
                    byte Race = (byte)ItemOrgTbl.getValue(i, 13);
                    byte Class = (byte)ItemOrgTbl.getValue(i, 14);
                    short Attack = (short)ItemOrgTbl.getValue(i, 15);
                    short Delay = (short)ItemOrgTbl.getValue(i, 0x10);
                    short Range = (short)ItemOrgTbl.getValue(i, 0x11);
                    short Weight = (short)ItemOrgTbl.getValue(i, 0x12);
                    short Duration = (short)ItemOrgTbl.getValue(i, 0x13);
                    int RepairPrice = (int)ItemOrgTbl.getValue(i, 20);
                    int SellingPrice = (int)ItemOrgTbl.getValue(i, 0x15);
                    short AC = (short)ItemOrgTbl.getValue(i, 0x16);
                    byte isCountable = (byte)ItemOrgTbl.getValue(i, 0x17);
                    uint Effect1 = (uint)ItemOrgTbl.getValue(i, 0x18);
                    uint Effect2 = (uint)ItemOrgTbl.getValue(i, 0x19);
                    sbyte ReqLevelMin = (sbyte)ItemOrgTbl.getValue(i, 0x1a);
                    sbyte ReqLevelMax = (sbyte)ItemOrgTbl.getValue(i, 0x1b);
                    byte ReqRank = (byte)ItemOrgTbl.getValue(i, 0x1c);
                    byte ReqTitle = (byte)ItemOrgTbl.getValue(i, 0x1d);
                    byte ReqStr = (byte)ItemOrgTbl.getValue(i, 30);
                    byte ReqHp = (byte)ItemOrgTbl.getValue(i, 0x1f);
                    byte ReqDex = (byte)ItemOrgTbl.getValue(i, 0x20);
                    byte ReqInt = (byte)ItemOrgTbl.getValue(i, 0x21);
                    byte ReqMp = (byte)ItemOrgTbl.getValue(i, 0x22);
                    byte SellingGroup = (byte)ItemOrgTbl.getValue(i, 0x23);
                    byte ItemClass = (byte)ItemOrgTbl.getValue(i, 0x24);
                    int NpBuyPrice = (int)ItemOrgTbl.getValue(i, 0x25);
                    short Bound = (short)ItemOrgTbl.getValue(i, 0x26);
                    byte UpgradeNotice = 0;


                    
                    TBL ExtTbl = tblArray[Extension];
                    for (int j = 0; j < ExtTbl.getRows(); j++)
                    {
                        uint baseItemID = (uint)ExtTbl.getValue(j, 2);
                        if ((baseItemID == ItemNum) || (baseItemID == 0)) 
                            {
                            string szHeader = "";
                            uint NewItemID = uint.Parse(ItemNum.ToString().Substring(0, 6) + ((uint)ExtTbl.getValue(j, 0)).ToString("000"));

                            if (baseItemID != 0)
                            {
                                szHeader = (string)ExtTbl.getValue(j, 1);
                            }
                            else if (((uint)ExtTbl.getValue(j, 0)) != 0)
                            {
                                szHeader = strName + "(+" + NewItemID.ToString().Substring(8) + ")";
                            }
                            else
                            {
                                szHeader = strName;
                            }
                            byte byItemType = (byte)ExtTbl.getValue(j, 7);
                            if (((byItemType != 4) || (ItemNum == baseItemID)) || (baseItemID == 0))
                            {
                                int BuyPrice = RepairPrice + (RepairPrice * int.Parse(NewItemID.ToString().Substring(8)));
                                short Damage = (short)(Attack + ((short)ExtTbl.getValue(j, 8)));
                                short HitRate = (short)ExtTbl.getValue(j, 10);
                                short EvosionRate = (short)ExtTbl.getValue(j, 11);
                                short TotalDuration = (short)(Duration + ((short)ExtTbl.getValue(j, 12)));
                                short TotalAc = (short)(AC + ((short)ExtTbl.getValue(j, 14)));
                                short DaggerAc = (short)ExtTbl.getValue(j, 15);
                                short JamadarAc = (short)ExtTbl.getValue(j, 0x10);
                                short SwordAc = (short)ExtTbl.getValue(j, 0x11);
                                short ClubAc = (short)ExtTbl.getValue(j, 0x12);
                                short AxeAc = (short)ExtTbl.getValue(j, 0x13);
                                short SpearAc = (short)ExtTbl.getValue(j, 20);
                                short ArrowAc = (short)ExtTbl.getValue(j, 0x15);
                                byte FireDamage = (byte)ExtTbl.getValue(j, 0x16);
                                byte GlacierDamage = (byte)ExtTbl.getValue(j, 0x17);
                                byte LightningDamage = (byte)ExtTbl.getValue(j, 0x18);
                                byte PoisonDamage = (byte)ExtTbl.getValue(j, 0x19);
                                byte HpRecovery = (byte)ExtTbl.getValue(j, 0x1a);
                                byte MPDamage = (byte)ExtTbl.getValue(j, 0x1b);
                                byte MpRecovery = (byte)ExtTbl.getValue(j, 0x1c);
                                byte RebelPhysDamage = (byte)ExtTbl.getValue(j, 0x1d);
                                byte Bind = (byte)ExtTbl.getValue(j, 0x1e);
                                short StrB = (short)ExtTbl.getValue(j, 0x1f);
                                short HpB = (short)ExtTbl.getValue(j, 0x20);
                                short DexB = (short)ExtTbl.getValue(j, 0x21);
                                short IntB = (short)ExtTbl.getValue(j, 0x22);
                                short MpB = (short)ExtTbl.getValue(j, 0x23);
                                short BonusHealt = (short)ExtTbl.getValue(j, 0x24);
                                short BonusMp = (short)ExtTbl.getValue(j, 0x25);
                                short FireResists = (short)ExtTbl.getValue(j, 0x26);
                                short IceResists = (short)ExtTbl.getValue(j, 0x27);
                                short LightningResists = (short)ExtTbl.getValue(j, 40);
                                short MagicResist = (short)ExtTbl.getValue(j, 0x29);
                                short PoisonResist = (short)ExtTbl.getValue(j, 0x2a);
                                short CurseResist = (short)ExtTbl.getValue(j, 0x2b);
                                sbyte ReqLevelMinTotal = (sbyte)(ReqLevelMin + ((short)ExtTbl.getValue(j, 0x2e)));
                                sbyte ReqLevelMaxTotal = (sbyte)(ReqLevelMax + ((short)ExtTbl.getValue(j, 0x2e)));
                                byte ReqStrTotal = (byte)(ReqStr + ((short)ExtTbl.getValue(j, 0x31)));
                                byte ReqHpTotal = (byte)(ReqHp + ((short)ExtTbl.getValue(j, 50)));
                                byte ReqDexTotal = (byte)(ReqDex + ((short)ExtTbl.getValue(j, 0x33)));
                                byte ReqIntTotal = (byte)(ReqInt + ((short)ExtTbl.getValue(j, 0x34)));
                                byte ReqMpTotal = (byte)(ReqMp + ((short)ExtTbl.getValue(j, 0x35)));

                                if (ReqLevelMinTotal < 0)
                                {
                                    ReqLevelMinTotal = 1;
                                }
                                if (ReqLevelMaxTotal < 0)
                                {
                                    ReqLevelMaxTotal = 1;
                                }
                                if (ReqStrTotal < 0)
                                {
                                    ReqStrTotal = 1;
                                }
                                if (ReqHpTotal < 0)
                                {
                                    ReqHpTotal = 1;
                                }
                                if (ReqDexTotal < 0)
                                {
                                    ReqDexTotal = 1;
                                }
                                if (ReqIntTotal < 0)
                                {
                                    ReqIntTotal = 1;
                                }
                                if (ReqMpTotal < 0)
                                {
                                    ReqMpTotal = 1;
                                }
                                if (szHeader.Length > 50)
                                {
                                    szHeader = szHeader.Substring(0, 50);
                                }
                                try
                                {
                                    using (OdbcCommand command = this.dbConnection.CreateCommand())
                                    {
                                        string Save = $"INSERT INTO ITEM (Num, Extension,strName, Description,isUnique,IconID, Kind, Slot, Race, Class, Damage, Delay, Range, Weight, Duration, BuyPrice, SellPrice, Ac, Countable, Effect1, Effect2, ReqLevel, ReqLevelMax, ReqRank, ReqTitle, ReqStr, ReqSta, ReqDex, ReqIntel, ReqCha, SellingGroup, ItemType, Hitrate, Evasionrate, DaggerAc,JamadarAc, SwordAc, MaceAc, AxeAc, SpearAc, BowAc, FireDamage, IceDamage, LightningDamage, PoisonDamage, HPDrain, MPDamage, MPDrain, MirrorDamage, Droprate, StrB, StaB, DexB, IntelB, ChaB, MaxHpB, MaxMpB, FireR, ColdR, LightningR, MagicR, PoisonR, CurseR,ItemClass,ItemExt,UpgradeNotice,NPbuyPrice,Bound) VALUES({NewItemID}, {Extension}, ?, ?, {isUnique},{dxtID}, {Kind}, {Slot}, {Race}, {Class}, {Damage}, {Delay}, {Range}, {Weight}, {TotalDuration}, {BuyPrice}, {SellingPrice}, {TotalAc}, {isCountable}, {Effect1}, {Effect2}, {ReqLevelMinTotal}, {ReqLevelMaxTotal}, {ReqRank}, {ReqTitle}, {ReqStrTotal}, {ReqHpTotal}, {ReqDexTotal}, {ReqIntTotal}, {ReqMpTotal}, {SellingGroup}, {byItemType}, {HitRate}, {EvosionRate}, {DaggerAc}, {JamadarAc}, {SwordAc},{ClubAc}, {AxeAc}, {SpearAc}, {ArrowAc}, {FireDamage}, {GlacierDamage}, {LightningDamage}, {PoisonDamage}, {HpRecovery}, {MPDamage}, {MpRecovery}, {RebelPhysDamage}, {Bind}, {StrB}, {HpB}, {DexB}, {IntB}, {MpB}, {BonusHealt}, {BonusMp}, {FireResists}, {IceResists}, {LightningResists}, {MagicResist}, {PoisonResist}, {CurseResist}, {ItemClass},{ItemExt},{UpgradeNotice}, {NpBuyPrice}, {Bound})"; 
                                        command.CommandText = Save;
                                        OdbcParameterCollection parameters = command.Parameters;
                                        parameters.Add(new OdbcParameter("", szHeader));
                                        parameters.Add(new OdbcParameter("", Description));
                                        command.Prepare();
                                        command.ExecuteNonQuery();
                                        command.Dispose();
                                    }
                                }
                                catch (OdbcException exception3)
                                {
                                    MessageBox.Show(exception3.Errors[0].Message, "Error");
                                    return;
                                }
                                num3++;
                                Application.DoEvents();
                            }
                        }
                    }
                    this.statusLabel.Text = "Importing items... " + num3;
                    Application.DoEvents();
                }
            }
            this.statusLabel.Text = "Finished importing " + num3 + " items!";
        }

        private void InitializeComponent()
        {
            this.btnImport = new Button();
            this.txtPath = new TextBox();
            this.btnBrowse = new Button();
            this.label1 = new Label();
            this.txtDSN = new TextBox();
            this.lblDSN = new Label();
            this.lblUID = new Label();
            this.txtUID = new TextBox();
            this.lblPWD = new Label();
            this.txtPWD = new TextBox();
            this.btnConnect = new Button();
            this.txtSuffix = new TextBox();
            this.lblSuffix = new Label();
            this.cboEncryption = new ComboBox();
            this.label2 = new Label();
            this.statusStrip1 = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();
            this.label3 = new Label();
            this.cboImport = new ComboBox();
            this.statusStrip1.SuspendLayout();
            base.SuspendLayout();
            this.btnImport.Location = new Point(0x131, 0x3d);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new Size(60, 0x15);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new EventHandler(this.btnImport_Click);
            this.txtPath.Location = new Point(0x4d, 0x21);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new Size(0x9d, 20);
            this.txtPath.TabIndex = 2;
            this.txtPath.TextChanged += new EventHandler(this.txtPath_TextChanged);
            this.btnBrowse.Location = new Point(240, 0x21);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new Size(0x1a, 0x13);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new EventHandler(this.btnBrowse_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 0x24);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x3e, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Data folder:";
            this.txtDSN.Location = new Point(0x26, 4);
            this.txtDSN.Name = "txtDSN";
            this.txtDSN.Size = new Size(0x3b, 20);
            this.txtDSN.TabIndex = 5;
            this.txtDSN.Text = "TEST";
            this.lblDSN.AutoSize = true;
            this.lblDSN.Location = new Point(4, 7);
            this.lblDSN.Name = "lblDSN";
            this.lblDSN.Size = new Size(30, 13);
            this.lblDSN.TabIndex = 7;
            this.lblDSN.Text = "DSN";
            this.lblUID.AutoSize = true;
            this.lblUID.Location = new Point(0x6a, 7);
            this.lblUID.Name = "lblUID";
            this.lblUID.Size = new Size(0x1a, 13);
            this.lblUID.TabIndex = 9;
            this.lblUID.Text = "UID";
            this.txtUID.Location = new Point(0x86, 4);
            this.txtUID.Name = "txtUID";
            this.txtUID.Size = new Size(0x3b, 20);
            this.txtUID.TabIndex = 8;
            this.txtUID.Text = "username";
            this.lblPWD.AutoSize = true;
            this.lblPWD.Location = new Point(0xcd, 7);
            this.lblPWD.Name = "lblPWD";
            this.lblPWD.Size = new Size(0x21, 13);
            this.lblPWD.TabIndex = 11;
            this.lblPWD.Text = "PWD";
            this.txtPWD.Location = new Point(240, 4);
            this.txtPWD.Name = "txtPWD";
            this.txtPWD.Size = new Size(0x3b, 20);
            this.txtPWD.TabIndex = 10;
            this.txtPWD.Text = "password";
            this.btnConnect.Location = new Point(0x131, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new Size(60, 20);
            this.btnConnect.TabIndex = 12;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new EventHandler(this.btnConnect_Click);
            this.txtSuffix.Location = new Point(0x14d, 0x21);
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new Size(0x20, 20);
            this.txtSuffix.TabIndex = 14;
            this.txtSuffix.Text = "_us";
            this.lblSuffix.AutoSize = true;
            this.lblSuffix.Location = new Point(0x111, 0x24);
            this.lblSuffix.Name = "lblSuffix";
            this.lblSuffix.Size = new Size(0x36, 13);
            this.lblSuffix.TabIndex = 15;
            this.lblSuffix.Text = "TBL suffix";
            this.cboEncryption.FormattingEnabled = true;
            this.cboEncryption.Items.AddRange(new object[] { "Standard", "New KOKO", "None" });
            this.cboEncryption.Location = new Point(0x4e, 0x3e);
            this.cboEncryption.Name = "cboEncryption";
            this.cboEncryption.Size = new Size(0x4c, 0x15);
            this.cboEncryption.TabIndex = 0x11;
            this.cboEncryption.Text = "Standard";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(12, 0x40);
            this.label2.Name = "label2";
            this.label2.Size = new Size(60, 13);
            this.label2.TabIndex = 0x12;
            this.label2.Text = "Encryption:";
            this.statusStrip1.Items.AddRange(new ToolStripItem[] { this.statusLabel });
            this.statusStrip1.Location = new Point(0, 0x57);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new Size(0x171, 0x16);
            this.statusStrip1.TabIndex = 0x13;
            this.statusStrip1.Text = "statusStrip1";
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(0, 0x11);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0xa3, 0x40);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x27, 13);
            this.label3.TabIndex = 0x15;
            this.label3.Text = "Import:";
            this.cboImport.FormattingEnabled = true;
            this.cboImport.Items.AddRange(new object[] { "Ext 0 -> 23", "Ext 0 -> 44" });
            this.cboImport.Location = new Point(0xd0, 0x3e);
            this.cboImport.Name = "cboImport";
            this.cboImport.Size = new Size(90, 0x15);
            this.cboImport.TabIndex = 20;
            this.cboImport.Text = "Ext 0 -> 23";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x171, 0x6d);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.cboImport);
            base.Controls.Add(this.statusStrip1);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.cboEncryption);
            base.Controls.Add(this.lblSuffix);
            base.Controls.Add(this.txtSuffix);
            base.Controls.Add(this.btnConnect);
            base.Controls.Add(this.lblPWD);
            base.Controls.Add(this.txtPWD);
            base.Controls.Add(this.lblUID);
            base.Controls.Add(this.txtUID);
            base.Controls.Add(this.lblDSN);
            base.Controls.Add(this.txtDSN);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.btnBrowse);
            base.Controls.Add(this.txtPath);
            base.Controls.Add(this.btnImport);
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.MaximizeBox = false;
            base.Name = "frmMain";
            base.SizeGripStyle = SizeGripStyle.Hide;
            this.Text = "v2 (twostars, 2009 - TheThyke, 2017)";
            base.Load += new EventHandler(this.frmMain_Load);
            base.FormClosing += new FormClosingEventHandler(this.frmMain_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void Query(string query)
        {
            using (OdbcCommand command = this.dbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
                command.Dispose();
            }
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
        }

        private void urlKODevs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.kodevs.com");
        }
    }
}

