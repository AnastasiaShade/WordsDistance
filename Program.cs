using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

//Выполнила: Стародубцева А. В.
//Группа: ПСм-11

//Задание: Алгоритмическая задача
//  Дан файл с текстом. Напишите функцию которая находит минимальное и максимальное расстояние между двумя словами.
//  Расстояние измеряется числом слов (одна буква тоже слово).

namespace AlgorithmTask {
    internal class Program {
        private const string SplitPattern = @"\W+";

        private static List<int> _firstWordPositions;
        private static List<int> _secondWordPositions;
        private static Decimal   _minDistance;
        private static Decimal   _maxDistance;

        /// <summary> Разбивает строку на слова </summary>
        /// <param name="source"></param>
        /// <returns>Возвращает список слов. Пустые элементы игнорируются.</returns>
        /// <exception cref="InvalidDataException">Выбрасывается, когда исходный текст пустой</exception>
        private static List<string> ParseStringToWords(string source) {
            var parsedData = Regex.Split(source, SplitPattern).ToList();

            for (var i = 0; i < parsedData.Count;) {
                if (string.IsNullOrEmpty(parsedData[i])) {
                    parsedData.RemoveAt(i);
                    continue;
                }

                ++i;
            }

            return parsedData;
        }
        
        /// <summary> Считывает данные из входного файла </summary>
        /// <returns> Возвращает список слов.</returns>
        /// <exception cref="ArgumentNullException"> Выбрасывается, когда имя файла не задано </exception>
        /// <exception cref="ArgumentException"> Выбрасывается, когда заданного файла не существует
        /// в указанной директории </exception>
        private static List<string> ReadWordsFromFile() {
            Console.Write("Please, enter input file name: ");
            var fileName = Console.ReadLine();

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName)) {
                throw new ArgumentNullException($"Invalid input file cant be null or empty.");
            }

            if (!File.Exists(fileName)) {
                throw new ArgumentException($"Input file \"{fileName}\" does not exist.");
            }
            
            //Все данные файла парсятся по символу конца строки, пустые строки автоматически удаляются
            var lines = File.ReadAllText(fileName)
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            
            //Строки разбиваются на слова
            var words = new List<string>();
            foreach (var line in lines) {
                words.AddRange(ParseStringToWords(line));
            }
            
            if (words.Count == 0) {
                throw new InvalidDataException("Source text cant be empty");
            }

            return words;
        }

        /// <summary> Получает слово из строки </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">Выбрасывается, когда в строке более 1 искомого слова</exception>
        private static string GetWordToFind(string source) {
            var parsedData = ParseStringToWords(source);

            if (parsedData.Count > 1) {
                throw new InvalidDataException("There are should be only one searching word in a string");
            }

            return parsedData[0].ToLower();
        }
        
        /// <summary> Рассчитывает минимальное и максимальное расстояние между искомыми словами </summary>
        /// <param name="source"> Текст, в котором будет происходить поиск слов </param>
        /// <param name="firstWord"> Первое искомое слово </param>
        /// <param name="secondWord"> Второе искомое слово </param>
        /// <exception cref="InvalidDataException">Выбрасывается, когда хотя бы одно слово не найдено в тексте</exception>
        private static void CalculateDistances(List<string> source, string firstWord, string secondWord) {
            var firstWordFound = false;
            var secondWordFound = false;
            var firstMin = -1;
            var secondMin = -1;
            var firstMax = -1;
            var secondMax = -1;
            
            for (var i = 0; i < source.Count; ++i) {
                var word = source[i].ToLower();

                if (!word.Equals(firstWord) && !word.Equals(secondWord)) {
                    continue;
                }
                
                if (word.Equals(firstWord)) {
                    firstMin = i;

                    if (!firstWordFound || secondWordFound && Math.Abs(i - secondMax) - 1 > _maxDistance) {
                        firstMax = i;
                    }
                    
                    firstWordFound = true;
                }

                if (word.Equals(secondWord)) {
                    secondMin = i;
                    
                    if (!secondWordFound || firstWordFound && Math.Abs(i - firstMax) - 1 > _maxDistance) {
                        secondMax = i;
                    }
                    
                    secondWordFound = true;
                }

                if (firstWordFound && secondWordFound) {
                    _minDistance = Math.Min(Math.Abs(firstMin - secondMin) - 1, _minDistance);
                    _maxDistance = Math.Max(Math.Abs(firstMax - secondMax) - 1, _maxDistance);
                }
            }

            if (!firstWordFound || !secondWordFound) {
                var word = !firstWordFound ? firstWord : secondWord;
                throw new InvalidDataException($"Source text does not contains \'{word}\'");
            }

            //Для случаев, когда искомые слова совпадают.
            _minDistance = Math.Max(0, _minDistance);
            _maxDistance = Math.Max(0, _maxDistance);
        }

        private static void Main() {
            try {
                var fileData = ReadWordsFromFile();
                
                Console.Write("First searching word: ");
                var firstWord = GetWordToFind(Console.ReadLine());
                Console.Write("Second searching word: ");
                var secondWord = GetWordToFind(Console.ReadLine());
                
                _maxDistance = Decimal.Zero;
                _minDistance = Decimal.MaxValue;

                CalculateDistances(fileData, firstWord, secondWord);
                    
                Console.WriteLine($"Min distance: {_minDistance}");
                Console.WriteLine($"Max distance: {_maxDistance}");
            }
            catch (Exception err) {
                Console.WriteLine(err);
            }
        }
    }
}