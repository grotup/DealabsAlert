using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealabsDatabase
{
    public class SQLiteDealabsData : IDealabsData<DealabsItem>
    {
        private string _DatabaseName;
        private string _TableName;
        private SQLiteConnection Connection;
        private string p1;
        private string p2;

        public SQLiteDealabsData(string DatabaseName, string TableName)
        {
            this._DatabaseName = DatabaseName;
            this._TableName = TableName;
            this.Connection = new SQLiteConnection(DatabaseName);

            CreateTable();
        }

        public void CreateTable()
        {
            this.Connection.CreateTable<DealabsItem>();
        }

        public DealabsItem get(int Id)
        {
            return this.Connection.Table<DealabsItem>().Where(T => T.Id == Id).First();
        }

        public List<DealabsItem> getAll()
        {
            return this.Connection.Table<DealabsItem>().Where(T => 1 == 1).ToList<DealabsItem>();
        }

        public void insert(DealabsItem Item)
        {
            this.Connection.Insert(Item);
        }

        public void delete(DealabsItem Item)
        {
            this.Connection.Delete(Item);
        }

        public void update(DealabsItem Item)
        {
            this.Connection.Update(Item);
        }
    }
}
