using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;

namespace MiraiEngine
{
    public static class SceneManager
    {
        public static string PathToScenes { get; set; }

        static SceneManager()
        {
            PathToScenes = @"Resources\Scenes\";
        }

        static public void SaveToFile(Scene scene, string filename)
        {
            var path = PathToScenes + filename;
            using(FileStream output = File.Open(path, FileMode.Create))
                SaveToStream(scene, output);
        }

        public static void SaveToStream(Scene scene, Stream output)
        {
            using (BinaryWriter writer = new BinaryWriter(output))
            {
                writer.Write(scene.Size.X);
                writer.Write(scene.Size.Y);
                foreach (var obj in scene)
                    WriteObject(writer, obj);
                writer.Close();
            }
        }

        static private void WriteObject(BinaryWriter writer, IGameObject obj)
        {
            writer.Write(obj.ID);
            writer.Write(obj.Body.Position.X);
            writer.Write(obj.Body.Position.Y);
        }

        static public Scene LoadFromFile(string filename)
        {
            var path = PathToScenes + filename;
            using (FileStream input = File.OpenRead(path))
                return LoadFromStream(input);
        }

        public static Scene LoadFromStream(Stream input)
        {
            using (BinaryReader reader = new BinaryReader(input))
            {
                Scene scene = new Scene(new Vector2f(reader.ReadSingle(), reader.ReadSingle()));
                while (reader.PeekChar() > -1)
                    scene.Add(BuildObject(scene, reader));
                reader.Close();
                if (scene.Camera == null) scene.CreateCamera();
                scene.Commit();
                return scene;
            }
        }

        private static GameObject BuildObject(Scene scene, BinaryReader reader)
        {
            var id = reader.ReadUInt32();
            var position = new Vector2f(reader.ReadSingle(), reader.ReadSingle());

            return ObjectsManager.Build(id, position);
        }
    }
}
