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
        static int Step = 0, Prev_step = 0, Tour = 0;
        static bool Is_first, Is_process;
        static List<string> TourText = new List<string>();
        private const string Text_For = "Вперед по маршруту";
        private const string Text_Back = "Назад по маршруту";
        private const string Text_1 = "Питер";
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
            Is_process = true;
            bot.StartReceiving();
            bot.OnMessage += MessageListener;
            Console.ReadKey();
        }
        [Obsolete]
        private static async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            //выпишем как бы лог по сообщениям, это можно и закомментировать
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
                            await bot.SendTextMessageAsync(e.Message.Chat.Id, s.Remove(0, 9), replyMarkup: ButtonsStart());
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
            if (message == "/download")
            {
                Prev_step = Step;
                Step = 6;
            }
                //скачиваем всю папку с загруженными объектами
                if (message == "/downloadall")
            {
                Is_process = false;
                string[] fileInput = Directory.GetFiles("DownLoad\\", "*.*");
                    //качаем все                   
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Качаю все");
                    foreach (string s in fileInput)
                        {
                            using (var st = File.Open(s, FileMode.Open))
                            {
                                InputOnlineFile file = new InputOnlineFile(st);
                                file.FileName = s.Remove(0, 9);
                                await bot.SendDocumentAsync(e.Message.Chat.Id, file);
                            }
                        }
                Is_process = true;
            }
            Is_first = false;
            string path;
            if (Is_process)
            {
                switch (Step)
                {
                    //Запуск и выбор города
                    case 0:
                        Is_first = true;
                        switch (message)
                        {
                            case "/start":
                                Is_process = false;
                                //отправим стикер и вводное сообщение
                                using (var s = File.OpenRead("Hello.webp"))
                                {
                                    await bot.SendStickerAsync(e.Message.Chat.Id, s);
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{e.Message.Chat.FirstName}, добро пожаловать в программу экскурсовода !", replyMarkup: ButtonsStart());
                                }
                                Is_process = true;
                                break;
                            case Text_1:
                                Tour = 1;
                                path = Read();
                                Is_process = false;
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step], replyMarkup: Buttons1());
                                }
                                Is_process = true;
                                ++Step;
                                break;
                            case Text_2:
                                Tour = 2;
                                path = Read();
                                Is_process = false;
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step], replyMarkup: Buttons1());
                                }
                                Is_process = true;
                                ++Step;
                                break;
                            case Text_3:
                                Tour = 3;
                                path = Read();
                                Is_process = false;
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step], replyMarkup: Buttons1());
                                }
                                Is_process = true;
                                ++Step;
                                break;
                            case Help:
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию. В каждой экскурсии по три точки, которые Вы посетите.", replyMarkup: ButtonsStart());
                                break;
                        }
                        break;
                    //Первый шаг экскурсий, карта города   
                    case 1:
                        switch (message)
                        {
                            case Text_For:
                                path = Read();
                                Is_process = false;
                                for (int i = 1; i <= 4; i++)
                                {
                                    using (var s = File.OpenRead(path + (Step).ToString() + "." + i.ToString() + ".jpg"))
                                    {
                                        await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                    }
                                }
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step], replyMarkup: Buttons());
                                Is_process = true;
                                ++Step;
                                break;
                            case Text_Back_All:
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                                --Step;
                                break;
                        }
                        break;
                    //Второй шаг экскурсий
                    case 2:
                        switch (message)
                        {
                            case Text_For:
                                Is_process = false;
                                path = Read();
                                for (int i = 1; i <= 3; i++)
                                {
                                    using (var s = File.OpenRead(path + (Step).ToString() + "." + i.ToString() + ".jpg"))
                                    {
                                        await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                    }
                                }
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step], replyMarkup: Buttons());
                                Is_process = true;
                                Step++;
                                break;
                            case Text_Back:
                                path = Read();
                                Is_process = false;
                                Step--;
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step-1], replyMarkup: Buttons1());
                                }
                                Is_process = true; 
                                break;
                            case Text_Back_All:
                                Step = 0;
                                Is_process = false;
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                                Is_process = true;
                                break;
                            case Text_Back_Tour:
                                Step = 1;
                                path = Read();
                                Is_process = false;
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step - 1], replyMarkup: Buttons1());
                                }
                                Is_process = true;
                                break;
                        }
                        break;
                    //Третий шаг экскурсий
                    case 3:
                        switch (message)
                        {
                            case Text_For:
                                Is_process = false;
                                path = Read();
                                for (int i = 1; i <= 3; i++)
                                {
                                    using (var s = File.OpenRead(path + (Step).ToString() + "." + i.ToString() + ".jpg"))
                                    {
                                        await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                    }
                                }
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step], replyMarkup: Buttons());
                                Step++;
                                Is_process = true;
                                break;
                            case Text_Back:
                                path = Read();
                                Is_process = false;
                                Step--;
                                for (int i = 1; i <= 3; i++)
                                {
                                    using (var s = File.OpenRead(path + (Step-1).ToString() + "." + i.ToString() + ".jpg"))
                                    {
                                        await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                    }
                                }
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step-1], replyMarkup: Buttons());    
                                Is_process = true;
                                break;
                            case Text_Back_All:
                                Step = 0;
                                Is_process = false;
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                                Is_process = true;
                                break;
                            case Text_Back_Tour:
                                Step = 1;
                                path = Read();
                                Is_process = false;
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step - 1], replyMarkup: Buttons1());
                                }
                                Is_process = true;
                                break;
                        }
                        break;
                    //Четвертый шаг экскурсий
                    case 4:
                        switch (message)
                        {
                            case Text_For:
                                Is_process = false;
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step], replyMarkup: ButtonsEnd());
                                Step++;
                                Is_process = true;
                                break;
                            case Text_Back:
                                path = Read();
                                Is_process = false;
                                Step--;
                                for (int i = 1; i <= 3; i++)
                                {
                                    using (var s = File.OpenRead(path + (Step-1).ToString() + "." + i.ToString() + ".jpg"))
                                    {
                                        await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), replyMarkup: Buttons());
                                    }
                                }
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, TourText[Step-1], replyMarkup: Buttons());
                                Is_process = true;
                                break;
                            case Text_Back_All:
                                Step = 0;
                                Is_process = false;
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                                Is_process = true;
                                break;
                            case Text_Back_Tour:
                                Step = 1;
                                path = Read();
                                Is_process = false;
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step - 1], replyMarkup: Buttons1());
                                }
                                Is_process = true;
                                break;
                        }
                        break;
                    //Последний шаг экскурcий
                    case 5:
                        switch (message)
                        {
                            case Text_Back_Tour:
                                Step = 1;
                                Is_process = false;
                                path = Read();
                                using (var s = File.OpenRead(path + "0.1.jpg"))
                                {
                                    await bot.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s), caption: TourText[Step - 1], replyMarkup: Buttons1());
                                }
                                Is_process = true;
                                break;
                            case Text_Back_All:
                                Step = 0;
                                Is_process = false;
                                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Выберите экскурсию", replyMarkup: ButtonsStart());
                                Is_process = true;
                                break;
                        }
                        break;
                    //скачивание файла
                    case 6:
                        string[] fileInput = Directory.GetFiles("DownLoad\\", "*.*");
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, "Напишите файл, который хотите скачать", replyMarkup: DownloadBot(fileInput));
                        Step++;
                        break;
                    case 7:
                        if(e.Message.Text == "Отмена")
                        {
                            Step = Prev_step;
                            switch (Step)
                            {
                                case 0:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: ButtonsStart());
                                    break;
                                case 1:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons1());
                                    break;
                                case 2:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons());
                                    break;
                                case 3:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons());
                                    break;
                                case 4:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons());
                                    break;
                                case 5:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: ButtonsEnd());
                                    break;
                            }
                            break;
                        }
                        using (var st = File.Open("DownLoad\\" + message, FileMode.Open))
                        {
                            InputOnlineFile file = new InputOnlineFile(st);
                            file.FileName = message;
                            await bot.SendDocumentAsync(e.Message.Chat.Id, file);
                            Step = Prev_step;
                            switch (Step)
                            {
                                case 0:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: ButtonsStart());
                                    break;
                                case 1:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons1());
                                    break;
                                case 2:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons());
                                    break;
                                case 3:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons());
                                    break;
                                case 4:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: Buttons());
                                    break;
                                case 5:
                                    await bot.SendTextMessageAsync(e.Message.Chat.Id, "Продолжим", replyMarkup: ButtonsEnd());
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Экскурсовод еще не закончил, подождите");
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
        /// создание inline клавиатуры для скачивания файлов
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup DownloadBot(string[] fileinfo)
        {

            var rk = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();
            foreach(string s in fileinfo)
            {
                cols.Add(new KeyboardButton(text: s.Remove(0, 9)));
                rows.Add(cols.ToArray());
                cols = new List<KeyboardButton>();
            }
            cols.Add(new KeyboardButton(text: "Отмена"));
            rows.Add(cols.ToArray());
            cols = new List<KeyboardButton>();
            rk.Keyboard = rows.ToArray();
            return rk;
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
