using System.Text.RegularExpressions;
//úkol - https://cw.fel.cvut.cz/wiki/courses/b0b99prpa/hw/hw07

// já fakt už netuším to je průšvih
//Došlo mi že už jsem rovnou mohl udělat třídu na Matici, kde by byl konstruktor List<List<int>>, ale v tomhle bodě už nehodlám přepisovat celý kód

List<List<List<int>>> Matice = new(); //víceméně List<List<int>> reprezentuje jednu matici :)
List<char> MaticeOp = new(); //tady ukládám operace mezi jednotlivými maticemi
char[] operatory = {'+', '-', '*'}; //dělení u matic není :) (naštěstí)

Console.WriteLine("Maticová kalkulačka :D\n(měla by zvládnout i víc než jeden výpočet)");
Console.WriteLine("(pro vyhodnocení rovnice nech prázdný řádek)\n---");
while (true)
{
    List<List<int>> smat = new(); //jako single matice (s mat)
    while (true)
    {
        string? line = Console.ReadLine();
        if (string.IsNullOrEmpty(line)) 
        {
            Matice.Add(smat);
            goto CALCULATE; //když uživatel dá prázdný řádek, tak už asi skončil, takže program začne vyhodnocovat matice :'(
        }

        List<int> tmp = new();
        foreach(string line_num in Regex.Replace(line, @"(\s{2,})", " ").Trim().Split(" ")) //kdyby se uživatel uklikl na mezerách mezi čísly
        {
            if (int.TryParse(line_num, out int num))
            {
                tmp.Add(num);
            }
            else if (operatory.Contains(line.Trim()[0])) //furt jenom [0] protože proč by to mělo jít jednoduše castnout na char >:(
            {
                MaticeOp.Add(line[0]);
                goto BREAK;
            }
        }

        if (tmp.Count > 1) smat.Add(tmp);
        else Console.WriteLine("Příliš krátký řádek >:( (2 čísla minimálně)");

        continue;
        BREAK:
            break;
    }
    Matice.Add(smat);
}

CALCULATE:
if (Matice.Count < 1 || MaticeOp.Count < 1 || Matice.Count == MaticeOp.Count)
{
    Console.WriteLine("\nMaticová rovnice je ve špatném formátu!)");
    Console.ReadLine();
    Environment.Exit(1);
}

//ten kód je hroznej, ale už s tím nic neudělám
do
{
    int multIndx = MaticeOp.IndexOf('*');
    if (multIndx != -1) //dává přednost násobícím operacím
    {
        List<List<int>>? tmp = VypocetMatice(Matice[multIndx], '*', Matice[multIndx + 1]);
        if (tmp == null) 
        {
            Console.WriteLine($"\nVyskytl se problém při výpočtu {multIndx + 1}. a {multIndx + 2}. matice! (buď nemají stejně prvků, nebo je výsledek příliš velký)"); // DODĚLAT UPŘEDNOSTNĚNÍ OPERANDŮ
            Console.ReadLine();
            Environment.Exit(1);
        }
        //vymění rovnice za právě vypočítanou
        MaticeOp.RemoveAt(multIndx);
        Matice.RemoveRange(multIndx, 2);
        Matice.Insert(multIndx, tmp);
    }
    else
    {
        for (int i = 0; i < MaticeOp.Count; i++)
        {
            List<List<int>>? tmp = VypocetMatice(Matice[i], MaticeOp[i], Matice[i + 1]);
            if (tmp == null) 
            {
                Console.WriteLine($"\nVyskytl se problém při výpočtu {i + 1}. a {i + 2}. matice! (buď nemají stejně prvků, nebo je výsledek příliš velký)"); // DODĚLAT UPŘEDNOSTNĚNÍ OPERANDŮ
                Console.ReadLine();
                Environment.Exit(1);
            }
            //vymění rovnice za právě vypočítanou
            MaticeOp.RemoveAt(i);
            Matice.RemoveRange(i, 2);
            Matice.Insert(i, tmp);
        }
    }
}
while (Matice.Count > 1);

//finální výpis
Console.WriteLine("\nVýsledek maticové rovnice:");
foreach (List<int> mat in Matice[0])
{
    Console.WriteLine(string.Join(' ', mat));
}

Console.ReadLine();



//pokud nastane chyba při výpočtu, vyhodí funkce null
static List<List<int>>? VypocetMatice(List<List<int>> mat1, char operand, List<List<int>> mat2)
{
    List<List<int>> vyslednaMatice = new();
    switch(operand) 
    {
        case '+':
            if (mat1.Count != mat2.Count) return null;
            for (int y = 0; y < mat1.Count; y++)
            {
                if (mat1[y].Count != mat2[y].Count) return null;
                List<int> tmp = new();
                for (int x = 0; x < mat1[y].Count; x++)
                {
                    try
                    {
                        checked //trochu zabezpečení
                        {
                            tmp.Add(mat1[y][x] + mat2[y][x]);
                        }
                    }
                    catch (OverflowException)
                    {
                        return null;
                    }
                    
                }
                vyslednaMatice.Add(tmp);
            }
            break;
        
        case '-':
            if (mat1.Count != mat2.Count) return null;
            for (int y = 0; y < mat1.Count; y++)
            {
                if (mat1[y].Count != mat2[y].Count) return null;
                List<int> tmp = new();
                for (int x = 0; x < mat1[y].Count; x++)
                {
                    try
                    {
                        checked
                        {
                            tmp.Add(mat1[y][x] - mat2[y][x]);
                        }
                    }
                    catch (OverflowException)
                    {
                        return null;
                    }
                }
                vyslednaMatice.Add(tmp);
            }
            break;

        case '*':
            //už vůbec nevím, jak se mi tohle podařilo vymyslet, takže doufám, že vy to taky nebudete chtít vědět :)
            if (mat1[0].Count != mat2.Count || mat1.Count != mat2[0].Count) return null;
            for (int y1 = 0; y1 < mat1.Count; y1++)
            {
                List<int> tmp = new();
                for (int y2 = 0; y2 < mat2[0].Count; y2++)
                {
                    int tmpPart = 0;
                    for (int x = 0; x < mat1[y1].Count; x++)
                    {
                        try
                        {
                            checked
                            {
                                tmpPart += mat1[y1][x] * mat2[x][y2];
                            }
                        }
                        catch (OverflowException)
                        {
                            return null;
                        }
                    }
                    tmp.Add(tmpPart);
                }
                vyslednaMatice.Add(tmp);
            }
            break;
        
        default:
            return null;
    }

    return vyslednaMatice;
}