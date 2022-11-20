namespace Controlwork_19_11_2022;

public class StringCut
{
    static public string Cut(string input)
    {
        int indexFirst = input.IndexOf("@#!elephant=&.ha-ha");
        int indexLast = input.IndexOf("</header>") - 1;
        input = input.Replace("header", "h2");
        
        for (int i = indexFirst; i <= indexLast; i++)
        {
            if (input[i] == '>')
            {
                indexFirst = i + 1;
                break;
            }
        }

        var temp_array = input.ToArray();
        for (int i = indexFirst; i <= indexLast; i++)
        {
            temp_array[i] = CaesarCode.CaesarCoding(temp_array[i]);
        }

        return string.Join("", temp_array);
    }
}