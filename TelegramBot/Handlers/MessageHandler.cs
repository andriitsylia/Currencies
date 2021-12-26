using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;
using TelegramBot.Services;
using TelegramBot.Settings;

namespace TelegramBot.Handlers
{
    public class MessageHandler
    {
        private static Banks _banks;
        private static Bank currentBank;
        private static DateTime currentDate;
        private static bool IsDateSelected;
        private static string currentCurrency;
        private static PrivatBankRatesSourceModel ratesSource;
        private static CurrencyListServiceModel currencyList;

        public static async Task Handler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            string[] command = messageText.Split(" ");

            switch (command[0])
            {
                case BotCommands.CMD_START:
                    await BotMessage.SendMessageMarkdownKeyboard(
                        botClient,
                        chatId,
                        "Hi\\! Let's go\\!\nSelect the *Bank*, the *Date* and the *Currency*",
                        ReplyKeyboard.MainKeyboard());

                    _banks = new BanksFromSettings().Get();
                    currentBank = null;
                    currentDate = DateTime.Today;
                    IsDateSelected = false;
                    currentCurrency = string.Empty;
                    break;

                case BotCommands.CMD_BANKSLIST:
                    await BotMessage.SendMessage(botClient, chatId, "Select the bank:\n");
                    await BotMessage.SendMessage(botClient, chatId, _banks.ToString());
                    await BotMessage.SendMessageMarkdown(botClient, chatId, "Type */bank* _bank_");
                    break;

                case BotCommands.CMD_BANK:
                    if (_banks == null)
                    {
                        await BotMessage.SendMessage(botClient, chatId, "Restart the bot\nType /start");
                        break;
                    }

                    if (command.Length < 2)
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "Parameter _bank_ is missing\nType */bank* _bank_");
                        break;
                    }

                    currentBank = _banks.Items.FirstOrDefault(b => b.Name.ToUpper() == command[1].ToUpper());

                    if (currentBank != null)
                    {
                        currentDate = DateTime.Today;
                        currentCurrency = string.Empty;
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "*" + currentBank.Name + "*\n\nType */date* _dd\\.mm\\.yyyy_");
                    }
                    else
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "I can't find the _" + command[1] + "_ bank");
                    }

                    break;

                case BotCommands.CMD_DATE:
                    if (currentBank == null)
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "Select the bank\nType */bank* _bank_");
                        break;
                    }

                    if (command.Length < 2)
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "Parameter _date_ is missing\nType */date* _date_");
                        break;
                    }

                    if (DateTime.TryParse(command[1], out currentDate))
                    {
                        currentCurrency = string.Empty;

                        ratesSource = JsonRatesParse.Parse(currentBank, currentDate);
                        currencyList = new CurrencyListServiceModel(ratesSource);

                        if (currencyList.Currencies.Count == 0)
                        {
                            await BotMessage.SendMessage(botClient, chatId, $"No currency rates on {currentDate.ToString(currentBank.DateFormat)}");
                            break;
                        }

                        IsDateSelected = true;
                        await BotMessage.SendMessageMarkdown(
                            botClient,
                            chatId,
                            $"Please, select any of the following currency :\n {string.Join(" ", currencyList.Currencies)}\n\nType */currency* _currency_");
                    }
                    else
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "The date isn't in _dd\\.mm\\.yyyy_ format");
                    }
                    break;

                case BotCommands.CMD_CURRENCY:
                    if (!IsDateSelected)
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "Enter the date\nType */date* _date_");
                        break;
                    }

                    if (command.Length < 2)
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "Сurrency isn't specified\nType */currency* _currency_");
                        break;
                    }

                    currentCurrency = currencyList?.Currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == command[1].ToUpper());

                    if (!string.IsNullOrWhiteSpace(currentCurrency))
                    {
                        CurrencyRateServiceModel currencyRate = new(ratesSource, currentCurrency);
                        ReportModel rep = new(currencyRate);
                        await BotMessage.SendMessage(botClient, chatId, rep.Report);
                    }
                    else
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "I can't find the _" + command[1] + "_ currency");
                    }
                    break;

                case BotCommands.BUTTON_BANK:
                    //currentCurrency = string.Empty;

                    CallbackQueryHandler._banks = new BanksFromSettings().Get();

                    await BotMessage.SendMessageKeyboard(
                        botClient,
                        chatId,
                        "Select the bank",
                        ReplyKeyboard.InlineBanksKeyboard(CallbackQueryHandler._banks));
                    break;

                case BotCommands.BUTTON_DATE:
                    if (CallbackQueryHandler.currentBank == null)
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "Bank isn't selected");
                        break;
                    }

                    CallbackQueryHandler.currentDate = DateTime.Today;
                    CallbackQueryHandler.currentCurrency = string.Empty;
                    await BotMessage.SendMessageKeyboard(
                        botClient,
                        chatId,
                        "Select the date, then press *Confirm date* button",
                        ReplyKeyboard.InlineDateKeyboard(CallbackQueryHandler.currentDate));
                    break;

                case BotCommands.BUTTON_CURRENCY:
                    if (!CallbackQueryHandler.IsDateSelected)
                    {
                        await BotMessage.SendMessageMarkdown(botClient, chatId, "Date isn't selected");
                        break;
                    }

                    CallbackQueryHandler.currentCurrency = string.Empty;
                    await BotMessage.SendMessageKeyboard(
                        botClient,
                        chatId,
                        "Select the currency",
                        ReplyKeyboard.InlineCurrencyKeyboard(CallbackQueryHandler.currencyList));
                    break;

                case BotCommands.CMD_HELP or BotCommands.BUTTON_HELP:
                default:
                    await CommandHelpHandler.Handler(botClient, update, cancellationToken);
                    break;
            }
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");
        }
    }
}
