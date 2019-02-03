using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Handlers
{
    public class SiteMap
    {
        [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
        public class Sitemap
        {
            private ArrayList map;

            public Sitemap()
            {
                map = new ArrayList();
            }

            [XmlElement("url")]
            public SitemapLocation[] Locations
            {
                get
                {
                    SitemapLocation[] items = new SitemapLocation[map.Count];
                    map.CopyTo(items);
                    return items;
                }
                set
                {
                    if (value == null)
                        return;
                    SitemapLocation[] items = (SitemapLocation[])value;
                    map.Clear();
                    foreach (SitemapLocation item in items)
                        map.Add(item);
                }
            }

            public string GetSitemapXml()
            {
                return string.Empty;
            }

            public int Add(SitemapLocation item)
            {
                return map.Add(item);
            }

            public void AddRange(IEnumerable<SitemapLocation> locs)
            {
                foreach (var i in locs)
                {
                    map.Add(i);
                }
            }

            public void WriteSitemapToFile(string path)
            {

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {

                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");

                    XmlSerializer xs = new XmlSerializer(typeof(Sitemap));
                    xs.Serialize(fs, this, ns);
                }
            }

            public string WriteSitemapToString()
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");

                    XmlSerializer xs = new XmlSerializer(typeof(Sitemap));
                    xs.Serialize(sw, this, ns);
                    return sw.GetStringBuilder().ToString();
                }
            }
        }
    }

    public class SitemapLocation
    {
        public enum eChangeFrequency
        {
            always,
            hourly,
            daily,
            weekly,
            monthly,
            yearly,
            never
        }

        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("changefreq")]
        public eChangeFrequency? ChangeFrequency { get; set; }
        public bool ShouldSerializeChangeFrequency() { return ChangeFrequency.HasValue; }

        [XmlElement("lastmod")]
        public DateTime? LastModified { get; set; }
        public bool ShouldSerializeLastModified() { return LastModified.HasValue; }

        [XmlElement("priority")]
        public double? Priority { get; set; }
        public bool ShouldSerializePriority() { return Priority.HasValue; }

        [XmlElement("image", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
        public List<SitemapImage> Images { get; set; }
        public bool ShouldSerializeImages() { return Images != null && Images.Count > 0; }
    }

    [XmlType(Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public class SitemapImage
    {
        [XmlElement("loc")]
        public string Location { get; set; }

        [XmlElement("caption")]
        public string Caption { get; set; }
        public bool ShouldSerializeCaption() { return !string.IsNullOrEmpty(Caption); }

        [XmlElement("title")]
        public string Title { get; set; }
        public bool ShouldSerializeTitle() { return !string.IsNullOrEmpty(Title); }

        [XmlElement("geo_location")]
        public string GeoLocation { get; set; }
        public bool ShouldSerializeGeoLoacation() { return !string.IsNullOrEmpty(GeoLocation); }
    }
}
