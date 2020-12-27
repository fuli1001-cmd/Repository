using Repository.SeedWork;

namespace Repository.UOW
{
    /// <summary>
    /// 实体改变跟踪类
    /// </summary>
    public class EntityTracker
    {
        public BaseEntity Entity { get; set; }

        public ChangeType changeType { get; set; }
    }

    public enum ChangeType
    {
        NoChange,
        Create,
        Update,
        Delete,
        DeleteCascade
    }
}
