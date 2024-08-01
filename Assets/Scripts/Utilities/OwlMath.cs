public static class OwlMath
{
	public static int Modulo(int value, int mod)
	{
		int result = value % mod;
		return result < 0 ? result + mod : result;
	}
}