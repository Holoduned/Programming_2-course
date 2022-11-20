namespace Controlwork_19_11_2022;

public class CaesarCode
{
    public static readonly Dictionary<char, char> alphabet = new Dictionary<char, char>()
    {
        {'a', 'â'},
        {'б', 'b'},
        {'в', 'v'},
        {'г', 'g'},
        {'д', 'đ'},
        {'е', 'ê'},
        {'ё', 'ê'},
        {'ж', 'g'},
        {'з', 'd'},
        {'и', 'i'},
        {'к', 'k'},
        {'л', 'l'},
        {'м', 'm'},
        {'н', 'n'},
        {'о', 'ô'},
        {'п', 'p'},
        {'р', 'r'},
        {'с', 's'},
        {'т', 't'},
        {'у', 'u'},
        {'ф', 'f'},
        {'х', 'h'},
        {'ц', 'c'},
        {'ч', 'h'},
        {'ш', 's'},
        {'щ', 's'},
        {'ъ', 'i'},
        {'ы', 'ư'},
        {'ь', 'i'},
        {'э', 'ê'},
        {'ю', 'u'},
        {'я', 'y'}
        
    };
    static public char CaesarCoding(char symbol)
    {
        if (!alphabet.ContainsKey(char.ToLower(symbol)))
        {
            return symbol;
        }
        var result = alphabet[char.ToLower(symbol)];
        if (char.IsUpper(symbol))
        {
            result = char.ToUpper(result);
        }

        return result;
    }
}