namespace com.armatur.common.serialization
{
    public interface ISerializerStringConverter
    {
        object ConvertFromString(string xml);

        string ConvertToString(object obj);
    }
}