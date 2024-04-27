#if true
using static WinLib.Util;
using LiteDB;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using static WinLib.LiteDBProps;
namespace WinLib;
public class LiteDBProps
{
    public class Prop
    {
        public long Id {
            get; set;
        }
        public string Name {
            get; set;
        }
        public object Data {
            get; set;
        }
    }
    private string filePath = null;
    //private LiteDatabase connection = null;
    //ILiteCollection<Prop> collection = null;
    public LiteDBProps(DirectoryInfo di)
    {
        this.filePath = Path.Combine(di.FullName, "properties.litedb");
        Dirs.PrepareForFile(this.filePath);
#if false
        this.connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        });
        this.collection = this.connection.GetCollection<Prop>("properties");
        // Nameをユニークインデックスにする
        this.collection.EnsureIndex(x => x.Name, true);
#endif
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            var collection = connection.GetCollection<Prop>("properties");
            // Nameをユニークインデックスにする
            collection.EnsureIndex(x => x.Name, true);
        }
    }
    public LiteDBProps(string orgName, string appNam) : this(new DirectoryInfo(Dirs.ProfilePath(orgName, appNam)))
    {
    }
    public dynamic? Get(string name)
    {
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            var result = collection.Find(x => x.Name == name).FirstOrDefault();
            connection.Commit();
            if (result == null) return null;
            return FromObject(result.Data);
        }
    }
    public void Put(string name, dynamic? data)
    {
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            var result = collection.Find(x => x.Name == name).FirstOrDefault();
            if (result == null)
            {
                result = new Prop {
                    Name = name, Data = ToObject(data)
                };
                collection.Insert(result);
                connection.Commit();
                return;
            }
            result.Data = ToObject(data);
            collection.Update(result);
            connection.Commit();
        }
    }
    public void Delete(string name)
    {
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            var result = collection.Find(x => x.Name == name).FirstOrDefault();
            if (result == null)
            {
                connection.Commit();
                return;
            }
            collection.Delete(result.Id);
            connection.Commit();
        }
    }
    public void DeleteAll()
    {
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            collection.DeleteAll();
            connection.Commit();
        }
    }
}
#endif
