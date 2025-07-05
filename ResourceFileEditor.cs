using System.Collections.Generic;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class ResourceFileEditor
    {
        public partial interface IResource
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string Path { get; set; }
        }
        
        public class Ctxr : IResource
        {
            
        }
        
        public class Evm : IResource
        {
            
        }
        
        public class Kms : IResource
        {
            
        }
        
        public class Cmdl : IResource
        {
            
        }
        
        public class BpAssetsFile
        {
            List<Ctxr> CtxrResources { get; set; }
            List<Cmdl> CmdlResources { get; set; }
        } 
        
        public class Tri : IResource
        {
            
        }
        
        public class Hzx : IResource
        {
            
        }
        
        public class Var : IResource
        {
            
        }
        
        public class Sar : IResource
        {
            
        }
        
        public class Row : IResource {}
        public class O2d : IResource{}
        public class Mar : IResource{}
        public class Lt2 : IResource{}
        public class Far : IResource{}
        public class Cv2 : IResource{}
        public class Anm : IResource { }
        public class Gcx : IResource{ }
        
        public class ManifestFile
        {
            public List<Tri> TriResources { get; set; }
            public List<Hzx> HzxResources { get; set; } 
            public List<Var> VarResources { get; set; } 
            public List<Sar> SarResources { get; set; } 
            public List<Row> RowResources { get; set; } 
            public List<O2d> O2dResources { get; set; } 
            public List<Mar> MarResources { get; set; } 
            public List<Lt2> Lt2Resources { get; set; } 
            public List<Kms> KmsResources { get; set; } 
            public List<Far> FarResources { get; set; } 
            public List<Evm> EvmResources { get; set; } 
            public List<Cv2> Cv2Resources { get; set; } 
            public List<Anm> AnmResources { get; set; } 
            public List<Gcx> GcxResources { get; set; } 
        }

        public class Object
        {
            BpAssetsFile bpAssetsFile { get; set; }
            ManifestFile manifestFile { get; set; }
            List<IResource> resourceList { get; set; }
            public string name { get; set; }
        }
    }
}