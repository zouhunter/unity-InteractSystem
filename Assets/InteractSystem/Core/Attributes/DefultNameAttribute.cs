
namespace InteractSystem.Attributes
{
    public class DefultNameAttribute : UnityEngine.PropertyAttribute
    {
        public string title;
        public string path;
        public DefultNameAttribute()
        {
            path = "name";
        }
        public DefultNameAttribute(string title)
        {
            path = "name";
            this.title = title;
        }
        public DefultNameAttribute(string title,string path)
        {
            this.title = title;
            this.path = path;
        }
    }
}