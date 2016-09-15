<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	var dt1 = DateTime.ParseExact("Ago/2012", "MMM/yyyy", CultureInfo.GetCultureInfo("pt-br")).Dump();
	var dt2 = DateTime.ParseExact("Jul/2016", "MMM/yyyy", CultureInfo.GetCultureInfo("pt-br")).Dump();
	
	dt2.Subtract(dt1).GetYears().Dump("Ano");
	dt2.AddDays(5).Subtract(dt1).GetMonthsRest().Dump("Mês");
}

public static class TimeSpanExtensions
{
	public static int GetYears(this TimeSpan timespan)
	{
		return (int)((double)timespan.Days / 365.2425);
	}
	public static int GetMonths(this TimeSpan timespan)
	{
		return (int)((double)timespan.Days / 30.436875);
	}
	public static int GetMonthsRest(this TimeSpan timespan)
	{
		int m = (int)((double)timespan.Days / 30.436875);
		int y = (int)((double)timespan.Days / 365.2425);
		m = m > 12 ? (m - (y * 12)) : m;
		return m;
	}
}
