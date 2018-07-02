
namespace InteractSystem.Attributes
{
    public class DefultNameAttribute : UnityEngine.PropertyAttribute
    {
        public string path;
        public DefultNameAttribute()
        {
            path = "name";
        }
        public DefultNameAttribute(string path)
        {
            this.path = path;
        }
    }
}