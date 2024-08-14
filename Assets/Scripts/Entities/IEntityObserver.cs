public interface IEntityObserver
{
	//List<EntitySubject> subjects = new List<EntitySubject>();
	public void OnNotify(EntityData data);

}