using com.armatur.common.enums;

namespace com.armatur.common.interfaces
{
    public interface IToggleable
    {
        ToggleState State { get; set; }
    }

    public interface IOpenable
    {
        bool Open();
        bool Close();
    }
}