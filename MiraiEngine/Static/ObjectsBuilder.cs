using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using System.IO;

namespace MiraiEngine
{
    public static class ObjectsManager
    {
        private static Dictionary<uint, Func<Vector2f, GameObject>> builder;
        public static Dictionary<uint, string> Registered { get; private set; }
        public static Dictionary<uint, string> BindedTextures { get; private set; }

        public static string PathToMOBs { get; set; }

        public static event Action<uint, string> NewTextureBinded;

        static ObjectsManager()
        {
            builder = new Dictionary<uint, Func<Vector2f, GameObject>>();
            Registered = new Dictionary<uint, string>();
            BindedTextures = new Dictionary<uint, string>();
            PathToMOBs = @"Resources\Objects\";
        }

        public static void Register(uint id, Vector2f size, double weight, Vector2f colliderOffset, 
            bool isSolid, bool isStatic, string texture, string filename)
        {
            if (id == 0) throw new ArgumentException("0x0000 is reserved ID.");
            
            Func<Vector2f, GameObject> constructor = position =>
                new GameObject(id, position, size, weight, colliderOffset, isSolid, isStatic);
            
            builder.Add(id, constructor);
            BindedTextures.Add(id, texture);

            if (NewTextureBinded != null) NewTextureBinded(id, texture);

            Registered.Add(id, filename.Substring(0, filename.Length - 4));
        }

        public static void LoadDefaultFolder()
        {
            var files = Directory.CreateDirectory(PathToMOBs).GetFiles();
            foreach (var file in files)
                LoadFromFile(file.Name);
        }

        public static void LoadFromFile(string filename)
        {
            var fullpath = PathToMOBs + filename;
            var reader = new BinaryReader(File.OpenRead(fullpath));

            var id = reader.ReadUInt32();
            var size = new Vector2f(reader.ReadSingle(), reader.ReadSingle());
            var colliderOffset = new Vector2f(reader.ReadSingle(), reader.ReadSingle());
            var isSolid = reader.ReadBoolean();
            var isStatic = reader.ReadBoolean();
            var weight = reader.ReadDouble();
            var texture = reader.ReadString();

            Register(id, size, weight, colliderOffset, isSolid, isStatic, texture, filename);
        }

        public static void SaveToFile(string filename, string textureName, GameObject obj)
        {
            var fullpath = PathToMOBs + filename;
            var writer = new BinaryWriter(File.OpenWrite(fullpath));
            var colliderOffset = obj.Body.Collider.Position - obj.Body.Position;

            writer.Write(obj.ID);
            writer.Write(obj.Body.Size.X);
            writer.Write(obj.Body.Size.Y);
            writer.Write(colliderOffset.X);
            writer.Write(colliderOffset.Y);
            writer.Write(obj.Body.IsSolid);
            writer.Write(obj.Body.IsStatic);
            writer.Write(obj.Body.Weight);
            writer.Write(textureName);
        }

        public static GameObject Build(uint id, Vector2f position)
        {
            try
            {
                return builder[id](position);
            }
            catch (KeyNotFoundException e)
            {
                throw new ArgumentException("ID not found: invalid ID token." + e.Message);
            }
        }
    }
}
