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
            Message sentMessage;
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            string[] command = messageText.Split(" ");

            switch (command[0])
            {
                case BotCommands.CMD_MODE:
                    await CommandModeHandler.Handler(botClient, update, cancellationToken);
                    break;

                case BotCommands.BUTTON_TEXT:
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Bot mode: " + BotCommands.BUTTON_TEXT,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);

                    await CommandHelpHandler.Handler(botClient, update, cancellationToken);

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Type */start*",
                        parseMode: ParseMode.MarkdownV2,
                        cancellationToken: cancellationToken);

                    break;

                case BotCommands.BUTTON_BUTTON:
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Bot mode: " + BotCommands.BUTTON_BUTTON,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken);

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Please, select the _Bank_, the_Date_ and the _Currency_",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: ReplyKeyboard.MainKeyboard(),
                        cancellationToken: cancellationToken);

                    _banks = null;
                    break;

                case BotCommands.CMD_START:
                    currentBank = null;
                    currentDate = DateTime.Today;
                    IsDateSelected = false;
                    currentCurrency = string.Empty;
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Please, type */bankslist* to output the banks list",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    break;

                case BotCommands.CMD_BANKSLIST:
                    _banks = new BanksListFromSettings().Get();

                    StringBuilder message = new();
                    message.Append("*Please, select the bank*:\n");

                    foreach (var bank in _banks.Items)
                    {
                        message.Append(bank.Name + "\n");
                    }
                    message.Append("\nType */bank* _bank_");

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message.ToString(),
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    break;

                case BotCommands.CMD_BANK:
                    if (_banks == null)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Restart the bot\nType /start",
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        break;
                    }

                    if (command.Length < 2)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Parameter _bank_ is missing\nType */bank* _bank_",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        break;
                    }

                    currentBank = _banks.Items.FirstOrDefault(b => b.Name.ToUpper() == command[1].ToUpper());

                    if (currentBank != null)
                    {
                        currentDate = DateTime.Today;
                        currentCurrency = string.Empty;

                        sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "*" + currentBank.Name + "*\n"
                            + "\nType */date* _dd\\.mm\\.yyyy_",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
                    }
                    else
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "I can't find the _" + command[1] + "_ bank",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }

                    break;

                case BotCommands.CMD_DATE:
                    if (currentBank == null)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Select the bank\nType */bank* _bank_",
                            parseMode: ParseMode.MarkdownV2,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        break;
                    }

                    if (command.Length < 2)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Parameter _date_ is missing\nType */date* _date_",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        break;
                    }

                    if (DateTime.TryParse(command[1], out currentDate))
                    {
                        currentCurrency = string.Empty;

                        ratesSource = JsonRatesParse.Parse(currentBank, currentDate);
                        currencyList = new CurrencyListServiceModel(ratesSource);

                        if (currencyList.Currencies.Count == 0)
                        {
                            sentMessage = await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "The " + currentBank.Name
                                   + " hasn't information about currency rates at "
                                   + currentDate.ToString(currentBank.DateFormat) + "\n"
                                   + "Select another date",
                               cancellationToken: cancellationToken);
                            break;
                        }

                        IsDateSelected = true;

                        sentMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Please, select any of the following currency :\n"
                                + string.Join(" ", currencyList.Currencies)
                                + "\n\nType */currency* _currency_",
                           parseMode: ParseMode.MarkdownV2,
                           replyMarkup: null,
                           cancellationToken: cancellationToken);
                    }
                    else
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "The date isn't in _dd\\.mm\\.yyyy_ format",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }
                    break;

                case BotCommands.CMD_CURRENCY:
                    if (!IsDateSelected)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Enter the date\nType */date* _date_",
                            parseMode: ParseMode.MarkdownV2,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        break;
                    }

                    if (command.Length < 2)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Сurrency isn't specified\nType */currency* _currency_",
                            replyToMessageId: update.Message.MessageId,
                            parseMode: ParseMode.MarkdownV2,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);

                        break;
                    }

                    currentCurrency = currencyList?.Currencies.Find(c => !string.IsNullOrWhiteSpace(c) && c.ToUpper() == command[1].ToUpper());

                    if (!string.IsNullOrWhiteSpace(currentCurrency))
                    {
                        CurrencyRateServiceModel currencyRate = new(ratesSource, currentCurrency);
                        ReportModel rep = new(currencyRate);

                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: rep.Report,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "I can't find the _" + command[1] + "_ currency",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: update.Message.MessageId,
                            replyMarkup: null,
                            cancellationToken: cancellationToken);
                    }
                    break;

                case BotCommands.BUTTON_BANK:
                    IsDateSelected = false;
                    currentCurrency = string.Empty;

                    CallbackQueryHandler._banks = new BanksListFromSettings().Get();

                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Select the bank",
                        replyMarkup: ReplyKeyboard.InlineBanksKeyboard(CallbackQueryHandler._banks),
                        cancellationToken: cancellationToken);

                    break;

                case BotCommands.BUTTON_DATE:
                    if (CallbackQueryHandler.currentBank == null)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Bank isn't selected",
                            cancellationToken: cancellationToken);
                        break;
                    }

                    CallbackQueryHandler.currentDate = DateTime.Today;
                    CallbackQueryHandler.currentCurrency = string.Empty;
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Select the date, then press *Confirm date* button",
                        parseMode: ParseMode.MarkdownV2,
                        replyMarkup: ReplyKeyboard.InlineDateKeyboard(CallbackQueryHandler.currentDate),
                        cancellationToken: cancellationToken);
                    break;

                case BotCommands.BUTTON_CURRENCY:
                    if (!CallbackQueryHandler.IsDateSelected)
                    {
                        sentMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Date isn't selected",
                            cancellationToken: cancellationToken);
                        break;
                    }

                    CallbackQueryHandler.currentCurrency = string.Empty;
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Select the currency",
                        replyMarkup: ReplyKeyboard.InlineCurrencyKeyboard(CallbackQueryHandler.currencyList),
                        cancellationToken: cancellationToken);
                    break;

                case BotCommands.CMD_HELP:
                default:
                    await CommandHelpHandler.Handler(botClient, update, cancellationToken);
                    break;
            }
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");
        }
    }
}
