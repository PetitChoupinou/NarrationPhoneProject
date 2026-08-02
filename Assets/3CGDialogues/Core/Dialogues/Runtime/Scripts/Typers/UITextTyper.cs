using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public class UITextTyper : MonoBehaviour, IUITextTyper
    {
        [SerializeField] private TextMeshProUGUI _textField;
        [SerializeField] private int _charactersPerSecond = 5;
        [SerializeField] private int _maxMsgWidth = 15;
        [SerializeField]private AudioClip _clip;
        [SerializeField]private AudioSource _source;
        [SerializeField] GameObject _panel;
        RectTransform _panelRect;
        Coroutine _endCoroutine;
        private int _clipIndex = 0;
        public int currentCharactersPerSeconds;
        public bool IsReadingText { get; private set; } = false;

#pragma warning disable 0414
        public bool IsWaitingForCommand { get; private set; } = false;
        private TextCommand _pendingCommand = null;
#pragma warning restore 0414

#pragma warning disable 0414
        private TextCommand[] _alwaysUpdatedCommand;
#pragma warning restore 0414

        private float _readCharacterOffset = 0;
        private int _readMaxCharacters = 0;

        private TextCommand[] _commands;

        public TextMeshProUGUI TextField => _textField;

        public string CurrentText { get; set; }
        public int CharactersPerSecond { get => _charactersPerSecond;}

        public TMP_Text _text;

        private void Awake()
        {
            currentCharactersPerSeconds = _charactersPerSecond;
        }
        private void Start()
        {
            _panelRect=_panel.GetComponent<RectTransform>();
            //ReadText("My name is <name=> ?");
        }
        public string AddLineReturn(string text)
        {
            int lastSpace = 0;
            int offset = 0;
            string returnText = text;
            int j = 1;
            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    j = 1;
                    continue;
                }
                if (text[i] == ' ')
                {
                    lastSpace = i;
                }
                if (j == _maxMsgWidth)
                {
                    //print("bitch" + text[i]);
                    if (lastSpace == 0 || lastSpace + _maxMsgWidth > i)
                    {
                        returnText = returnText.Insert(lastSpace + offset + 1, "\n");
                    }
                    else
                    {
                        returnText = returnText.Insert(i + offset + 1, "\n");
                    }
                    offset += 1;
                    j = 0;
                }
                j++;
            }
            return returnText;
        }
     

        public void ReadText(string text )
        {
            CurrentText= text;
            _panelRect.localScale = Vector3.one;
            _text.text = "";
            CurrentText = AddLineReturn(CurrentText);
            if (_endCoroutine != null)
            {
                StopCoroutine(_endCoroutine);
                _endCoroutine = null;
            }
            StartCoroutine(ReadingText());
        }
        IEnumerator ReadingText()
        {
            yield return null;
            _clipIndex = 0;
            if (_commands != null)
            {
                foreach (TextCommand command in _commands)
                {
                    command.Release();
                }
            }

            _commands = _GenerateCommands(CurrentText);

            CurrentText = _RemoveCustomTags(CurrentText);
            TextField.text = CurrentText;
            TextField.ForceMeshUpdate();
            _readCharacterOffset = 0f;
            _readMaxCharacters = TextField.GetParsedText().Length;

            foreach (TextCommand command in _commands)
            {
                command.OnReadStart();

            }

            IsReadingText = true;

        }
        public void GoToEnd()
        {
            if (!IsReadingText) return;
            IsReadingText = false;
            TextField.maxVisibleCharacters = _readMaxCharacters;
            foreach (TextCommand command in _commands) {
                command.OnReadEnd();
            }
            _endCoroutine=StartCoroutine(EndCoroutine());
        }
        IEnumerator EndCoroutine()
        {
            yield return new WaitForSeconds(3);

            _panelRect.localScale = Vector3.zero;
            yield return null;
        }
        private void Update()
        {
            if (IsReadingText)
            {
                _UpdateReadText();
                _UpdateAlwaysUpdatedCommands();
            }
            
        }

        private void _UpdateAlwaysUpdatedCommands()
        {
            foreach(TextCommand command in _commands)
            {
                if (command.AlwaysUpdated)
                    command.OnUpdate();
            }
        }

        private void _UpdateReadText()
        {
            if (!IsReadingText) return;

            if (IsWaitingForCommand)
            {
                if (_pendingCommand != null && _pendingCommand.IsRunning)
                {
                    _pendingCommand.OnUpdate();
                    return;
                }
                else
                {
                    IsWaitingForCommand = false;
                }
            }





            float startOffset = _readCharacterOffset;
            float endOffset = startOffset + _charactersPerSecond * Time.deltaTime;

            int startIndex = Mathf.FloorToInt(startOffset);
            int endIndex = Mathf.FloorToInt(endOffset);
            
            if (endIndex > _clipIndex+1)
            {
                if (_clip != null)
                {
                    _source.PlayOneShot(_clip);
                }
                _clipIndex = endIndex;
            }
            TextCommand[] commandsToEnter = TextCommandUtils.FindCommandsToEnter(_commands, startIndex, endIndex);
            foreach (TextCommand command in commandsToEnter)
            {
                command.OnEnter();

                if (command.IsBlocking && _pendingCommand != command)
                {
                    _pendingCommand = command;
                    IsWaitingForCommand = true;
                }


            }

            TextCommand[] commandsToExit = TextCommandUtils.FindCommandsToExit(_commands, startIndex, endIndex);
            foreach (TextCommand command in commandsToExit)
            {
                command.OnExit();

            }

            _GoToCharacter(endOffset);
            if (_readCharacterOffset >= _readMaxCharacters)
            {
                GoToEnd();
            }
        }

        private void _GoToCharacter(float characterOffset)
        {
            _readCharacterOffset = characterOffset;
            _text.text = CurrentText.Substring(0, (int)_readCharacterOffset);
        }

        private  TextCommand[] _GenerateCommands(string text)
        {
            int startIndex = 1;
            int offset = 0;
            TextCommandsFactory factory = new TextCommandsFactory();
            List<TextCommand> commands = new List<TextCommand>();
            //TODO: Copy from Exercise 2 + Manage closing tags
            //Example <camshake=0.2>BOO!</camshake> instead of <camshake=0.2|0.1>BOO!
            for (int i = 0; i < text.Length; ++i)
            {
                char character = text[i];
                if (character == '<')
                {
                    startIndex = i;
                }
                else if (character == '>')
                {
                    string tagName = TagsUtils.ExtractTagName(text.Substring(startIndex, i - startIndex));
                    string tagArg = TagsUtils.ExtractTagArgs(text.Substring(startIndex, i - startIndex));
                    if (TagsUtils.IsCustomTag(tagName))
                    {
                        if (text[startIndex + 1] == '/')
                        {
                            if (tagArg != "")
                            {
                                TextCommand command = factory.CreateCommand(tagName);
                                command.Init(this);                               
                                command.TagName = tagName;
                                command.EnterIndex = startIndex - offset;
                                command.SetupData(tagArg);
                                commands.Add(command);
                            }
                            else
                            {
                                commands.FindLast(x => x.TagName == tagName).ExitIndex = startIndex - 1 - offset;
                            }
                        }
                        else
                        {
                            TextCommand command = factory.CreateCommand(tagName);
                            command.Init(this);
                            command.EnterIndex = startIndex - offset;
                            command.TagName = tagName;
                            command.SetupData(tagArg);
                            commands.Add(command);
                        }
                    }
                    offset += i - startIndex + 1;
                }
            }
            return commands.ToArray();
        }

        private static string _RemoveCustomTags(string text)
        {
            int startIndex = 0;

            //TODO: Copy From Exercise 2
            for (int i = 0; i < text.Length; ++i)
            {
                char character = text[i];
                switch (character)
                {
                    case '<':
                        startIndex = i;
                        break;
                    case '>':
                        string tagName = TagsUtils.ExtractTagName(text.Substring(startIndex, i - startIndex));
                        string tagArg = TagsUtils.ExtractTagArgs(text.Substring(startIndex, i - startIndex));
                        if (text[startIndex + 1] != '/')
                        {
                            text = text.Replace("<" + tagName + "=" + tagArg + ">", "");
                        }
                        else
                        {
                            text = text.Replace("</" + tagName + ">", "");
                        }
                        i = 0;
                        break;

                }
            }
            return text;
        }
    }
}