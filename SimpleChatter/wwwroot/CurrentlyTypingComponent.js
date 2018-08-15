function CurrentlyTypingComponent($form,$typingStatus)
{
	let $el = $typingStatus;
	var _timeout;
	var _names = [];

	this.start = connection =>
	{
		connection.on('StartedTyping', name =>
		{
			console.log(name + ' started typing');
			_names.push(name);
			updateDisplay();
		});

		connection.on('StoppedTyping', name =>
		{
			let ix = _names.indexOf(name);
			if (ix == -1)
			{
				return;
			}
			_names.splice(ix, 1);
			updateDisplay();
		});

		$form.submit(stoppedTyping);
		$form.keydown(e =>
		{
			switch (e.which)
			{
				//Meta keys by themselves don't count as "typing"
				case 16: //Shift
				case 17: //Ctrl
				case 18: //Alt
					return;
			}
			if (_timeout)
			{
				clearTimeout(_timeout);
				console.log('still typing.  timeout reset');
			} else
			{
				connection.send('startedTyping');
				console.log('started typing.  timeout set');
			}
			_timeout = setTimeout(stoppedTyping, 1000);
		});

		function stoppedTyping() {
			if (!_timeout)
			{
				return;
			}
			clearTimeout(_timeout);
			_timeout = undefined;
			connection.send('StoppedTyping');
			console.log('typing stopped');
		}
	};

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