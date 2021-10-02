using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;

namespace HomeWork_9
{
    class Program
    {
        static int Step = 0, Tour = 0;
        static bool Is_first;
        static List<string> TourText = new List<string>();
        private const string Text_For = "Вперед по маршруту";
        private const string Text_Back = "Назад по маршруту";
        private const string Text_1 = "Санкт-Петербург";
        private const string Text_2 = "Москва";
        private const string Text_3 = "Лондон";
        private const string Help = "Помощь";
        private const string Text_Back_Tour = "Вернуться к началу экскурсии";
        private const string Text_Back_All = "Вернуться к выбору экскурсий";
        static TelegramBotClient bot;

        [Obsolete]
        static void Main(string[] args)
        {
            //запишем токен и запустим бота
            string token = File.ReadAllText("token bot.txt");
            bot = new TelegramBotClient(token);
            bot.StartReceiving();
            bot.OnMessage += MessageListener;
            Console.ReadKey();
        }
        [Obsolete]
        private static async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
                string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            //выпишем как бы лог по сообщения, это можно и закомментировать
            string message = e.Message.Text;
            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");
                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
                {
                    Console.WriteLine(e.Message.Document.FileId);
                    Console.WriteLine(e.Message.Document.FileName);
                    Console.WriteLine(e.Message.Document.FileSize);
                    Download(e.Message.Document.FileId, e.Message.Document.FileName);
                }
            //обработка сообщений
            if (message == null) return;
            if (message == "/start") 
                Step = 0;
            //просмотр загруженных объектов
            if (message == "/list")
            {
                string[] fileInput = Directory.GetFiles("DownLoad\\", "*.*");
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Количество файлов: {fileInput.Length}", replyMarkup: ButtonsStart()); 
                foreach (string s in fileInput)
                {
                    switch (Step)
                    {
                        case 0:
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, s.Remove(0,9), replyMarkup: ButtonsStart());
                            break;
                        case 1:
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, s.Remove(0, 9), replyMarkup: Buttons1());
                            break;
                        case 2:
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, s.Remove(0, 9), replyMarkup: Buttons());
                            break;
                        case 3:
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, s.Remove(0, 9), replyMarkup: Buttons());
                            break;
                        case 4:
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, s.Remove(0, 9), replyMarkup: Buttons());
                            break;
                        case 5:
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, s.Remove(0, 9), replyMarkup: ButtonsEnd());
                            break;
                    }
                }
            };
            //скачивание папки с загруженными объектами
            if (message == "/download")
            {
                string[] fileInput = Directory.GetFiles("DownLoad\\", "*.*");
                foreach (string s in fileInput)
                    {
                        using (var st = File.OpenRead(s))
                        { 
                            await bot.SendDocumentAsync(e.Message.Chat.Id, st,caption: s.Remove(0, 9));
                        }
                    }
            }
                Is_first = false;
            string path=null;
            switch (Step)
            {
                //Запуск
                case 0:
                    Is_first = true;
                    switch (message)
                    {
                        case "/start":
                            //отправим стикер и вводное сообщение
                            using (var s = File.OpenRead("Hello.webp"))
                            {
                                await bot.SendStickerAsync(e.Message.Chat.Id, s);
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Добро пожаловать в программу экскурсовода!", replyMarkup: ButtonsStart());
                            }
                                break;
                        case Text_1:
                            Tour = 1;
                            ++Step;
                            path = Read();
                            using (var s = File.OpenRead(path + "0.1.jpg"))
                            {
                                await bot.SendPhotoAsync(e.Message.Chat.Id,new Telegram.Bot.Types.InputFiles.InputOnlineFile(s) ,caption: TourText[Step - 1], replyMarkup: Buttons1());
                            }
                            break;
                        case Text_2:
                            Tour = 2;
                            ++Step;
                            path = Read();
                            using (var s = File.OpenRead(path + "0.1.jpg"))
                            {
                                await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step - 1], replyMarkup: Buttons1());
                            }
                            break;
                        case Text_3:
                            Tour = 3;
                            ++Step;
                            path = Read();
                            using (var s = File.OpenRead(path + "0.1.jpg"))
                            {
                                await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step - 1], replyMarkup: Buttons1());
                            }
                            break;
                        case Help:
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию. В каждой экскурсии по три точки, которые Вы посетите.", replyMarkup: ButtonsStart());
                            break;
                    }
                    break;
                //Первый шаг экскурсий    
                case 1:
                    switch (message)
                    {
                        case Text_For:
                            ++Step;
                            path = Read();
                            for (int i=1; i<=4; i++)
                            {
                                using (var s = File.OpenRead(path + (Step - 1).ToString()+ "."+i.ToString()+".jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                }
                            }
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step - 1], replyMarkup: Buttons());
                            break;
                        case Text_Back_All:
                            --Step;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                            break;
                    }
                    break;
                //Второй шаг экскурсий
                case 2:
                    switch (message)
                    {
                        case Text_For:
                            Step++;
                            path = Read();
                            for (int i = 1; i <= 3; i++)
                            {
                                using (var s = File.OpenRead(path + (Step - 1).ToString() + "." + i.ToString() + ".jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                }
                            }
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step - 1], replyMarkup: Buttons());
                            break;
                        case Text_Back:
                            Step--;
                            path = Read();
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step - 1], replyMarkup: Buttons1());
                            break;
                        case Text_Back_All:
                            Step = 0;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                            break;
                        case Text_Back_Tour:
                            Step = 1;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Начинаем экскурсию заново", replyMarkup: Buttons1());
                            break;
                    }
                    break;
                //Третий шаг экскурсий
                case 3:
                    switch (message)
                    {
                        case Text_For:
                            Step++;
                            path = Read();
                            for (int i = 1; i <= 3; i++)
                            {
                                using (var s = File.OpenRead(path + (Step - 1).ToString() + "." + i.ToString() + ".jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                }
                            }
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step - 1], replyMarkup: Buttons());
                            break;
                        case Text_Back:
                            Step--;
                            path = Read();
                            for (int i = 1; i <= 3; i++)
                            {
                                using (var s = File.OpenRead(path + (Step - 1).ToString() + "." + i.ToString() + ".jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                }
                            }
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step - 1], replyMarkup: Buttons());
                            break;
                        case Text_Back_All:
                            Step = 0;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                            break;
                        case Text_Back_Tour:
                            Step = 1;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Начинаем экскурсию заново", replyMarkup: Buttons1());
                            break;
                    }
                    break;
                //Четвертый шаг экскурсий
                case 4:
                    switch (message)
                    {
                        case Text_For:
                            Step++;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step - 1], replyMarkup: ButtonsEnd());
                            break;
                        case Text_Back:
                            Step--;
                            path = Read();
                            for (int i = 1; i <= 3; i++)
                            {
                                using (var s = File.OpenRead(path + (Step - 1).ToString() + "." + i.ToString() + ".jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                }
                            }
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step - 1], replyMarkup: Buttons());
                            break;
                        case Text_Back_All:
                            Step = 0;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                            break;
                        case Text_Back_Tour:
                            Step = 1;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Начинаем экскурсию заново", replyMarkup: Buttons1());
                            break;
                    }
                    break;
                //Последний шаг экскурcий
                case 5:
                    switch (message)
                    {
                        case Text_Back_Tour:
                            Step=1;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Начинаем экскурсию заново", replyMarkup: Buttons1());
                            break;
                        case Text_Back_All:
                            Step = 0;
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// создание inline клавиатуры для 2,3 этапа экскурсий
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup Buttons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton { Text = Text_For },
                        new KeyboardButton { Text = Text_Back }, 
                    },
                    new List<KeyboardButton> {
                        new KeyboardButton { Text = Text_Back_All },
                        new KeyboardButton { Text = Text_Back_Tour },
                    }
                }
            };
        }
        /// <summary>
        /// создание inline клавиатуры для экскурсий для 1 этапа экскурсии
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup Buttons1()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton { Text = Text_For },
                        new KeyboardButton { Text = Text_Back_All },
                    }
                }
            };
        }
        /// <summary>
        /// создание inline клавиатуры перед выбором экскурсий
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup ButtonsStart()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton { Text = Text_1 },
                        new KeyboardButton { Text = Text_2 },
                        new KeyboardButton { Text = Text_3 },
                        new KeyboardButton { Text = Help },
                    }
                }
            };
        }
        /// <summary>
        /// создание inline клавиатуры для конца маршрута
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup ButtonsEnd()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton { Text = Text_Back_Tour },
                        new KeyboardButton { Text = Text_Back_All },
                    }
                }
            };
        }
        /// <summary>
        /// Считываение речи экскурсовода
        /// </summary>
        /// <returns></returns>
        private static string Read()
        {
            string path = "";
            switch (Tour)
            {
                case 1:
                    path = "Санкт-Петербург\\";
                    break;
                case 2:
                    path = "Москва\\";
                    break;
                case 3:
                    path = "Лондон\\";
                    break;
                default:
                    break;
            }
            if (Is_first)
            {
                TourText.Clear();
                using (StreamReader sr = new StreamReader(path+"Tour.txt", System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        TourText.Add(line);
                    }
                }
            }
            //return TourText[Step-1];
            return path;
        }
        /// <summary>
        /// Скачивание файлов
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="path"></param>
        static async void Download(string fileID, string path)
        {
            var file = await bot.GetFileAsync(fileID);
            FileStream fs = new FileStream("DownLoad\\" + path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }
    }
}
