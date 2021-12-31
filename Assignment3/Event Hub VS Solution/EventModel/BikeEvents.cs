using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventModel
{
    public class BikeEvents
    {
        private int marker = 0;
        private int endMarker = 0;

        private List<BikeData> bikeDataCollection = new List<BikeData>();
        public string FileName
        {
            get;
            set;
        }

        public void Initialize(string file)
        {
            FileName = file;
            marker = 0;
            bikeDataCollection.Clear();

            if (!File.Exists(FileName))
            {
                throw new Exception(String.Format("File {0} does not exist", FileName));
            }

            StreamReader reader = new StreamReader(FileName);
            string line;

           // while ((line = reader.ReadLine()) != null)
            //{
                //var bikedata = jsonconvert.deserializeobject<bikedata>(line);
                //bikedatacollection.add(bikedata);
              //  endMarker += 1;
            //}
            using (StreamReader r = new StreamReader(FileName))
            {   // read the json file as a whole
                string json = r.ReadToEnd();
                List<BikeData> items = JsonConvert.DeserializeObject<List<BikeData>>(json);
                bikeDataCollection = items;
                endMarker = bikeDataCollection.Count ;

            }
        }

        public List<BikeData> GetBatch(int size = 20)
        {
            var list = new List<BikeData>();
            int end = ((marker + size) > endMarker) ? endMarker : marker + size;

            for (var i = marker; i < end; i++)
            {
                list.Add(bikeDataCollection[i]);
            }
            // shift the marker to send the next batch
            marker = marker + size; 
            return list;
        }
    }
}
