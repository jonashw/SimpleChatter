function CurrentlyTypingComponent(userName, $form, chat)
{
	console.log(userName, $form, chat);
	let $el = $("#typingStatus");
	var _timeout;
	var _names = [];

	console.log('starting to listen: ', chat.client);

	chat.client.startedTyping = name =>
	{
		if (name == userName)
		{
			return;
		}
		console.log(name + ' started typing');
		_names.push(name);
		updateDisplay();
	};

	chat.client.stoppedTyping = name =>
	{
		console.log(name + ' stopped typing');
		let ix = _names.indexOf(name);
		if (ix == -1)
		{
			return;
		}
		_names.splice(ix, 1);
		updateDisplay();
	};

	this.start = () =>
	{
		$form.submit(stoppedTyping);
		$form.keydown(() =>
		{
			if (_timeout)
			{
				clearTimeout(_timeout);
				console.log('still typing.  timeout reset');
			} else
			{
				chat.server.startedTyping(userName);
				console.log('started typing.  timeout set');
			}
			_timeout = setTimeout(stoppedTyping, 1000);
		});
	};

	function stoppedTyping() {
		if (!_timeout)
		{
			return;
		}
		clearTimeout(_timeout);
		_timeout = undefined;
		chat.server.stoppedTyping(userName);
		console.log('typing stopped');
	}

	function updateDisplay() {
		switch (_names.length)
		{
			case 0:
				$el.text('').removeClass('mt-1');
				break;
			case 1:
				$el.text(_names[0] + ' is typing').addClass('mt-1');
				break;
			default:
				$el.text('Typing: ' + _names.join(', ')).addClass('mt-1');
				break;
		}
	}
}