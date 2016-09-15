<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	GetPeriodo("Ago/2012", "Abr/2016").Dump();
}

// Define other methods and classes here
public string GetPeriodo(string dataInicio, string dataTermino)
{
	DateTime dt1 = DateTime.ParseExact(dataInicio, "MMM/yyyy", CultureInfo.GetCultureInfo("pt-br"));
	DateTime dt2 = DateTime.ParseExact(dataTermino, "MMM/yyyy", CultureInfo.GetCultureInfo("pt-br"));
	TimeSpan timespan = dt2.AddDays(5).Subtract(dt1);
	int m = (int)((double)timespan.Days / 30.436875);
	int y = (int)((double)timespan.Days / 365.2425);
	m = m > 12 ? (m - (y * 12)) : m;

	return m == 0
					? String.Format("{0} {1}", y, y > 1 
														? "anos" 
														: "ano")
			 : y > 0
					? String.Format("{0} {1}, {2} {3}", y, y > 1 
																	? "anos" 
																	: "ano", 
														   m, m > 1
														   			? "meses"
																	: "mês")
					: String.Format("{0} {1}", m, m > 1 ? "meses" : "mês");
}