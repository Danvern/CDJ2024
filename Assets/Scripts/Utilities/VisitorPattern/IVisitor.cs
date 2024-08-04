public interface IVisitor
{
	void Visit(IMovementLogic visitable);
	void Visit(EntityHealthLogic visitable);
	void Visit(Entity visitable);
}