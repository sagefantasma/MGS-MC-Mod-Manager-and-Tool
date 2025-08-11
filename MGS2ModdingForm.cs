using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MGS2ModdingForm : Form
    {
        private ConfigSettings config;
        private RichTextBox modInfoRichTextBox;
        private FlowLayoutPanel modListPanel;
        private FileExplorerManager fileExplorerManager;
        private ModListManager modListManager;

        private readonly string[] expectedPaths = new string[]
        {
            "eu/stage/d001p01",
            "eu/stage/d005p03",
            "assets/gcx/us/_bp",
            "assets/gcx/eu/_bp",
            "assets/hzx/eu",
            "assets/hzx/us",
            "assets/mar/us",
            "assets/evm/us",
            "assets/evm/us/_win",
            "assets/kms/us",
            "assets/kms/us/_win",
            "textures/flatlist/_win",
            "textures/flatlist/ovr_eu/_win",
            "textures/flatlist/ovr_PS3/_win",
            "textures/flatlist/ovr_stm/_win",
            "textures/flatlist/ovr_stm/ovr_eu/_win",
            "textures/flatlist/ovr_stm/ctrltype_kbd/_win",
            "textures/flatlist/ovr_stm/ctrltype_nx",
            "textures/flatlist/ovr_stm/ctrltype_ps4/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps4/ovr_eu/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps4/ovr_jp/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps5/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps5/ovr_eu/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps5/ovr_jp/_win",
            "textures/flatlist/ovr_stm/ctrltype_stmd/_win",
            "textures/flatlist/ovr_stm/ctrltype_xs/_win",
            "C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/codec",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/demo",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/demo2",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/movie",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/movievr",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/vox",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/vox2",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/codec/_bp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/demo/_bp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/demo2/_bp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/capture",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f00a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f01b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f01c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f01d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f01e",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f01f",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f03b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f04a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f04b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f04c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f04d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f04e",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f05a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/f06a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/mobile",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/node",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/face/photo",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/movie/_bp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/movievr/_bp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a00a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a00b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a00c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a01b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a01c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a01d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a01e",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a01f",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a02b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a03b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a12a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a12b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a13a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a13b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a13c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a14a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a14b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a15a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a15b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a16a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a17a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a18a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a19a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a20a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a20b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a20c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a20e",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a21a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a21b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a22a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a22b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a23a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a23b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a24a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a24b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a24c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a24d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a24f",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a24g",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a25a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a25d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a28a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a31a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a31b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a31c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a41a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a41b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a42a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a43a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a45a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a46a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/a61a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/boss",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d001p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d001p02",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d005p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d005p03",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d00t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d010p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d012p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d014p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d01t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d021p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d036p03",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d036p05",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d045p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d046p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d04t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d053p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d055p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d05t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d063p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d065p02",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d070p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d070p09",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d070px9",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d078p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d080p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d080p06",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d080p07",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d080p08",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d082p01",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d10t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d11t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d12t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d12t3",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d12t4",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d13t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/d14t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ending",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/init",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/mselect",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/museum",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/n_title",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt0",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt1",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt10",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt11",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt12",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt13",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt2",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt3",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt4",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt5",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt6",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt7",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt8",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt9",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_plt_s",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_rai_b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_sna_b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_title",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_tnk0",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_tnk_r",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_1",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_p",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_r",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_rp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_s",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_sp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_t",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/r_vr_x",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/select",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp06a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp07a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp08a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp21a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp22a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp24a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sp25a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/sselect",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/st01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/st02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/st03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/st04a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/st05a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta00a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta01b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta01c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta01d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta01e",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta01f",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta12a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta20a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta22a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta24a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta31a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/ta42a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/tales",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/tsp03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/tvs03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/tvs05a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/tvs06a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/tvs08a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/twp03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/twp34a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/twp43a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs04a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs05a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs06a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs07a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs08a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs09a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/vs10a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w00a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w00b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w00c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w01b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w01c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w01d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w01e",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w01f",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w03b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w04a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w04b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w04c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w11a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w11b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w11c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w12a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w12b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w12c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w13a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w13b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w14a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w15a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w15b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w16a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w16b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w17a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w18a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w19a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w20a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w20b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w20c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w20d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w21a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w21b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w22a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w23a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w23b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w24a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w24b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w24c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w24d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w24e",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w25a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w25b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w25c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w25d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w28a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w31a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w31b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w31c",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w31d",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w31f",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w32a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w32b",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w41a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w42a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w43a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w44a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w45a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w46a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w51a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/w61a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/webdemo",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wmovie",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp01a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp02a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp03a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp04a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp05a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp11a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp12a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp13a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp14a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp15a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp21a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp22a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp23a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp24a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp25a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp31a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp32a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp33a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp34a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp35a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp41a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp42a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp43a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp44a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp45a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp51a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp52a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp53a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp54a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp55a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp61a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp62a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp63a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp64a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp65a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp71a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp72a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp73a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp74a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/stage/wp75a",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/vox/_bp",
"C:/Program Files (x86)/Steam/steamapps/common/MGS2/eu/vox2/_bp"
        };

        public MGS2ModdingForm()
        {
            InitializeComponent();

            modInfoRichTextBox = new RichTextBox
            {
                Multiline = true,
                ReadOnly = true,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(15, 57, 48),
                ScrollBars = RichTextBoxScrollBars.None,
                WordWrap = true,
                MaximumSize = new Size(364, 133),
                Size = new Size(364, 133),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
                ForeColor = SystemColors.Control,
            };
            this.Controls.Add(modInfoRichTextBox);

            this.FormClosing += new FormClosingEventHandler(MGS2ModdingForm_FormClosing);
        }

        private void MGS2ModdingForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
            config = ConfigManager.LoadSettings();

            if (!CheckAndPromptForFolderPaths())
                return;

            string gameInstallPath = config.GamePaths["MGS2"];
            if (string.IsNullOrEmpty(gameInstallPath) || !Directory.Exists(gameInstallPath))
            {
                gameInstallPath = FindMGS2Installation();
                if (!string.IsNullOrEmpty(gameInstallPath))
                {
                    config.GamePaths["MGS2"] = gameInstallPath;
                    ConfigManager.SaveSettings(config);
                }
            }

            modListPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                Size = new Size((int)(this.Width / 1.5), this.Height - 80 - 80),
                Location = new Point(this.Width - (int)(this.Width / 1.5) - 50, 80),
                BackColor = ColorTranslator.FromHtml("#0f3930"),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 10, 20, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            this.Controls.Add(modListPanel);

            fileExplorerManager = new FileExplorerManager(config, this, modListPanel, "MGS2", expectedPaths);
            fileExplorerManager.SetupBackupFolders();
            fileExplorerManager.SetupModFolder();

            modListManager = new ModListManager(modListPanel);

            LoadModsIntoUI();

            if (!config.Backup.MGS2BackupCompleted)
            {
                fileExplorerManager.BackupVanillaFiles(gameInstallPath);
                config.Backup.MGS2BackupCompleted = true;
                ConfigManager.SaveSettings(config);
            }
        }

        #region First Time Setup

        private bool CheckAndPromptForFolderPaths()
        {
            if (!config.MGS2VanillaFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Before you can modify the files we need to make a backup of your MGS2 Files.\n\nDo you want to use the default location for the MGS2 Vanilla Files folder?\n\nClick 'No' if you'd like to select your own location" +
                    "\nDefault location:\n" + config.MGS2VanillaFolderPath,
                    "Vanilla Files Location", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel)
                {
                    GoBackToMainMenu();
                    return false;
                }
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.MGS2VanillaFolderPath,
                        Description = "Select a folder where the 'MGS2 Vanilla Files' folder will be created."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.MGS2VanillaFolderPath = Path.Combine(fbd.SelectedPath, "MGS2 Vanilla Files");
                        }
                        else
                        {
                            GoBackToMainMenu();
                            return false;
                        }
                    }
                }
                config.MGS2VanillaFolderSet = true;
            }

            if (!config.MGS2ModFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Now we need to set up a location where your mods will be stored.\n\nDo you want to use the default location for the MGS2 Mods folder?\n\nClick 'No' if you'd like to select your own location" +
                    "\nDefault location:\n" + config.MGS2ModFolderPath,
                    "MGS2 Mods Folder Location", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel)
                {
                    GoBackToMainMenu();
                    return false;
                }
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.MGS2ModFolderPath,
                        Description = "Select a folder where the 'MGS2 Mods' folder will be created."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.MGS2ModFolderPath = Path.Combine(fbd.SelectedPath, "MGS2 Mods");
                        }
                        else
                        {
                            GoBackToMainMenu();
                            return false;
                        }
                    }
                }
                config.MGS2ModFolderSet = true;
            }
            ConfigManager.SaveSettings(config);
            return true;
        }

        private string FindMGS2Installation()
        {
            string[] commonPaths = {
                @"C:\Program Files (x86)\Steam\steamapps\common\MGS2",
                @"A:\SteamLibrary\steamapps\common\MGS2",
                @"B:\SteamLibrary\steamapps\common\MGS2",
                @"D:\SteamLibrary\steamapps\common\MGS2",
                @"E:\SteamLibrary\steamapps\common\MGS2",
                @"F:\SteamLibrary\steamapps\common\MGS2",
                @"G:\SteamLibrary\steamapps\common\MGS2",
            };

            string foundPath = commonPaths.FirstOrDefault(Directory.Exists);
            if (!string.IsNullOrEmpty(foundPath))
            {
                return foundPath;
            }
            else
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the installation folder for MGS2 - Master Collection.";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        return fbd.SelectedPath;
                    }
                    else
                    {
                        GoBackToMainMenu();
                        return null;
                    }
                }
            }
        }

        private void GoBackToMainMenu()
        {
            LoggingManager.Instance.Log("User did not complete folder selection. Returning to Main Menu.\n");
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.FormClosing -= MGS2ModdingForm_FormClosing;
            this.Close();
        }

        #endregion

        #region Mod List GUI Setup

        private void LoadModsIntoUI()
        {
            if (!Directory.Exists(fileExplorerManager.ModFolder))
                return;
            string[] modDirs = Directory.GetDirectories(fileExplorerManager.ModFolder);
            List<ModListManager.ModItem> modItems = new List<ModListManager.ModItem>();

            foreach (string modPath in modDirs)
            {
                string modName = Path.GetFileName(modPath);
                bool isEnabled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
                bool isHDfix = fileExplorerManager.IsMGSHDFixMod(modPath);
                modItems.Add(new ModListManager.ModItem
                {
                    ModName = modName,
                    ModPath = modPath,
                    IsEnabled = isEnabled,
                    IsHDfix = isHDfix
                });
            }

            Point savedScroll = modListManager.ModListPanel.AutoScrollPosition;

            modListManager.LoadMods(modItems,
                onToggle: ToggleModAction,
                onRename: RenameModAction,
                onDelete: DeleteModAction,
                onSettings: SettingsAction,
                onHoverEnter: (modName, ctrl) => ShowModImage(modName, ctrl),
                onHoverLeave: () => HideModImage());

            modListManager.ModListPanel.AutoScrollPosition = new Point(-savedScroll.X, -savedScroll.Y);


        }

        #endregion

        #region Delegate Callback Methods

        private async void ToggleModAction(string modName)
        {
            string gameInstallPath = config.GamePaths["MGS2"];
            if (!Directory.Exists(gameInstallPath))
            {
                MessageBox.Show(
                    "Game installation not found, cannot apply mods.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // special-case FPS-Unlock
            if (modName.Equals("MGSFPSUnlock", StringComparison.OrdinalIgnoreCase))
            {
                bool isEnabled = config.Mods.ActiveMods.TryGetValue(modName, out var on) && on;
                string fpsFolder = Path.Combine(config.MGS2ModFolderPath, modName);

                if (isEnabled)
                    MGSFPSUnlockManager.Disable(gameInstallPath, config);
                else
                    MGSFPSUnlockManager.Enable(fpsFolder, gameInstallPath, config);

                LoadModsIntoUI();
                return;
            }

            try
            {
                Point savedScroll = modListManager.ModListPanel.AutoScrollPosition;
                await fileExplorerManager.ToggleModStateByNameAsync(modName, gameInstallPath);
                LoadModsIntoUI();
                modListManager.ModListPanel.AutoScrollPosition = new Point(-savedScroll.X, -savedScroll.Y);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void RenameModAction(string modName)
        {
            RenameModByName(modName);
            LoadModsIntoUI();
        }

        private void DeleteModAction(string modName)
        {
            Button dummy = new Button { Tag = modName };
            fileExplorerManager.DeleteMod(dummy, EventArgs.Empty);
            LoadModsIntoUI();
        }

        private void SettingsAction(string modName)
        {
            string modPath = Path.Combine(fileExplorerManager.ModFolder, modName);
            string iniPath = Path.Combine(modPath, "MGSHDFix.ini");
            if (!File.Exists(iniPath))
            {
                MessageBox.Show("MGSHDFix.ini not found in mod folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MGSHDFixSettingsForm settingsForm = new MGSHDFixSettingsForm("MGS2", this);
            settingsForm.ShowDialog();
            this.ActiveControl = modListManager.ModListPanel;
        }


        private void RenameModByName(string modName)
        {
            Button dummy = new Button { Tag = modName };
            RenameMod(dummy, EventArgs.Empty);
        }

        #endregion

        #region Hover Methods

        private void ShowModImage(string modName, Control modControl)
        {
            string modImagePath = Path.Combine(fileExplorerManager.ModFolder, modName, "Mod Details", "Mod Image.png");
            string modInfoPath = Path.Combine(fileExplorerManager.ModFolder, modName, "Mod Details", "Mod Info.txt");
            if (File.Exists(modImagePath))
                hoverPictureBox.Image = Image.FromFile(modImagePath);
            else
                hoverPictureBox.Image = null;
            hoverPictureBox.Size = new Size(364, 270);
            hoverPictureBox.Location = new Point(14, 223);
            hoverPictureBox.Visible = true;
            hoverPictureBox.BringToFront();
            ShowModInfo(modInfoPath);
        }

        private void ShowModInfo(string modInfoPath)
        {
            if (File.Exists(modInfoPath))
                modInfoRichTextBox.Text = File.ReadAllText(modInfoPath);
            else
                modInfoRichTextBox.Text = string.Empty;
            modInfoRichTextBox.Location = new Point(12, hoverPictureBox.Bottom);
            modInfoRichTextBox.Visible = true;
            modInfoRichTextBox.BringToFront();
        }

        private void HideModImage()
        {
            if (hoverPictureBox.Image != null)
            {
                hoverPictureBox.Image.Dispose();
                hoverPictureBox.Image = null;
            }
            hoverPictureBox.Visible = false;
            modInfoRichTextBox.Visible = false;
        }

        #endregion

        #region Mod Renaming and Editing

        private void RenameMod(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            string oldModName = button.Tag.ToString();
            string oldModPath = Path.Combine(fileExplorerManager.ModFolder, oldModName);
            string newModPath = oldModPath;
            if (MessageBox.Show("Do you want to rename the mod?", "Rename Mod", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string newModName = PromptForModName(oldModName);
                if (string.IsNullOrWhiteSpace(newModName) || newModName == oldModName)
                    return;
                newModPath = Path.Combine(fileExplorerManager.ModFolder, newModName);
                if (Directory.Exists(newModPath))
                {
                    MessageBox.Show($"A mod with the name '{newModName}' already exists.", "Rename Mod", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    Directory.CreateDirectory(newModPath);
                    Directory.CreateDirectory(Path.Combine(newModPath, "Mod Details"));
                    foreach (string dirPath in Directory.GetDirectories(oldModPath, "*", System.IO.SearchOption.AllDirectories))
                    {
                        string newDirPath = dirPath.Replace(oldModPath, newModPath);
                        if (!newDirPath.EndsWith("Mod Details", StringComparison.OrdinalIgnoreCase))
                            Directory.CreateDirectory(newDirPath);
                    }
                    foreach (string filePath in Directory.GetFiles(oldModPath, "*.*", System.IO.SearchOption.AllDirectories))
                    {
                        string newFilePath = filePath.Replace(oldModPath, newModPath);
                        if (!newFilePath.Contains(Path.Combine(newModPath, "Mod Details")))
                            File.Move(filePath, newFilePath);
                    }
                    FileSystem.DeleteDirectory(oldModPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    UpdateModNameInConfig(oldModName, newModName);
                    oldModPath = newModPath;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access denied while renaming mod '{oldModName}':\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renaming mod '{oldModName}':\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            string modDetailsPath = Path.Combine(oldModPath, "Mod Details");
            if (!Directory.Exists(modDetailsPath))
                Directory.CreateDirectory(modDetailsPath);
            if (MessageBox.Show("Do you want to select a new mod image?", "Select Mod Image", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                SelectModImage(modDetailsPath);
            if (MessageBox.Show("Do you want to edit the mod description?", "Edit Mod Description", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                EditModDescription(modDetailsPath);
            LoadModsIntoUI();
        }

        private string PromptForModName(string currentName)
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 150;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Rename Mod";
                prompt.StartPosition = FormStartPosition.CenterParent;
                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Enter a new name for the mod:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, Text = currentName };
                Button confirmation = new Button() { Text = "OK", Left = 280, Width = 80, Top = 80, DialogResult = DialogResult.OK };
                prompt.AcceptButton = confirmation;
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                return (prompt.ShowDialog() == DialogResult.OK) ? textBox.Text.Trim() : null;
            }
        }

        private void SelectModImage(string modDetailsPath)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Select a Mod Image";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedImagePath = ofd.FileName;
                    string destinationImagePath = Path.Combine(modDetailsPath, "Mod Image.png");
                    File.Copy(selectedImagePath, destinationImagePath, true);
                }
            }
        }

        private void EditModDescription(string modDetailsPath)
        {
            string modInfoPath = Path.Combine(modDetailsPath, "Mod Info.txt");
            using (Form prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 300;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Edit Mod Description";
                prompt.StartPosition = FormStartPosition.CenterParent;
                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Enter a new description for the mod:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, Height = 150, Multiline = true, ScrollBars = ScrollBars.Vertical };
                if (File.Exists(modInfoPath))
                    textBox.Text = File.ReadAllText(modInfoPath);
                Button confirmation = new Button() { Text = "OK", Left = 280, Width = 80, Top = 220, DialogResult = DialogResult.OK };
                prompt.AcceptButton = confirmation;
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                if (prompt.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(modInfoPath, textBox.Text.Trim());
            }
        }

        private void UpdateModNameInConfig(string oldModName, string newModName)
        {
            bool isActive = config.Mods.ActiveMods.ContainsKey(oldModName) && config.Mods.ActiveMods[oldModName];
            config.Mods.ActiveMods.Remove(oldModName);
            config.Mods.ActiveMods[newModName] = isActive;
            if (config.Mods.ModMappings.ContainsKey(oldModName))
            {
                var mappings = config.Mods.ModMappings[oldModName];
                config.Mods.ModMappings.Remove(oldModName);
                config.Mods.ModMappings[newModName] = mappings;
            }
            if (config.Mods.ReplacedFiles.ContainsKey(oldModName))
            {
                var replacedFiles = config.Mods.ReplacedFiles[oldModName];
                config.Mods.ReplacedFiles.Remove(oldModName);
                config.Mods.ReplacedFiles[newModName] = replacedFiles;
            }
            ConfigManager.SaveSettings(config);
        }

        #endregion

        #region Other Event Handlers

        private void MGS2ModdingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");
            Application.Exit();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Going back to Main Menu from MGS2.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS2ModdingForm");
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide();
        }

        private void RefreshMods_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Refreshing mods list in MGS2 form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS2ModdingForm");
            MGS2ModdingForm newForm = new MGS2ModdingForm();
            newForm.Show();
            this.Hide();
        }

        private void MoveVanillaFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = config.MGS2VanillaFolderPath,
                Description = "Select a new location for the MGS2 Vanilla Files folder."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string newFolderPath = Path.Combine(fbd.SelectedPath, "MGS2 Vanilla Files");
                    try
                    {
                        Directory.Move(config.MGS2VanillaFolderPath, newFolderPath);
                        fileExplorerManager.SetupBackupFolders();
                        config.MGS2VanillaFolderPath = newFolderPath;
                        ConfigManager.SaveSettings(config);
                        MessageBox.Show("Vanilla Files folder moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error moving Vanilla Files folder:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RebuildVanillaFiles_Click(object sender, EventArgs e)
        {
            string gameInstallPath = config.GamePaths["MGS2"];
            if (string.IsNullOrEmpty(gameInstallPath) || !Directory.Exists(gameInstallPath))
            {
                MessageBox.Show("Game installation not found. Please set the game path first.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            fileExplorerManager.BackupVanillaFiles(gameInstallPath);
            config.Backup.MGS2BackupCompleted = true;
            ConfigManager.SaveSettings(config);
            MessageBox.Show("Vanilla files have been rebuilt successfully.",
                "Rebuild Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MoveMgs2ModFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = config.MGS2ModFolderPath,
                Description = "Select a new location for the MGS2 Mods folder."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string newFolderPath = Path.Combine(fbd.SelectedPath, "MGS2 Mods");
                    try
                    {
                        Directory.Move(fileExplorerManager.ModFolder, newFolderPath);
                        config.MGS2ModFolderPath = newFolderPath;
                        fileExplorerManager.SetupModFolder();
                        ConfigManager.SaveSettings(config);
                        MessageBox.Show("MGS2 Mods folder moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error moving MGS2 Mods folder:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void AddMods_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you adding a single mod?",
                "Add Mod(s)", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return;
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = fileExplorerManager.ModFolder,
                Description = (result == DialogResult.Yes) ?
                    "Select the mod folder you want to add." :
                    "Select the folder containing the mods you want to add."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    if (result == DialogResult.Yes)
                        fileExplorerManager.ProcessModFolder(fbd.SelectedPath);
                    else if (result == DialogResult.No)
                    {
                        string[] modDirs = Directory.GetDirectories(fbd.SelectedPath);
                        if (modDirs.Length == 0)
                        {
                            MessageBox.Show("The selected folder does not contain any mod folders.",
                                "No Mods Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        foreach (string modPath in modDirs)
                        {
                            fileExplorerManager.ProcessModFolder(modPath);
                        }
                    }
                    ConfigManager.SaveSettings(config);
                    LoadModsIntoUI();
                }
            }
        }

        #endregion
    }
}
