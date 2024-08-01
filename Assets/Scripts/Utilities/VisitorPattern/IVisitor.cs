public interface IVisitor
{
	void Visit(MovementLogic visitable);
	void Visit(EntityHealthLogic visitable);
	void Visit(Entity visitable);
}