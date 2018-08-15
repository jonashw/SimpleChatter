function ChatMessagesComponent($log, $form, $input, $currentName, $changeNameBtn) {
	$input.focus();
	this.start = connection =>
	{
		var currentName = "";

		connection.on('MessagePosted', (name, message) => {
			writeMessage(
				$('<strong style="margin-right:4px;"></strong>').text(name),
				$('<span></span>').text(' ' + message));
		});

		connection.on('UserNameGiven', userName => {
			currentName = userName;
			writeMessage($('<em></em>').text('Welcome! Your auto-assigned user name is ' + userName));
			updateCurrentNameDisplay();
		});

		connection.on('NameChanged', (oldName,newName) => {
			writeMessage($('<em></em>').text(oldName + ' shall now be known as ' + newName));
		});

		connection.on('Joined', name => {
			writeMessage($('<em></em>').text(name + ' has joined'));
		});

		connection.on('Left', name => {
			writeMessage($('<em></em>').text(name + ' has departed'));
		});

		$changeNameBtn.click(() =>
		{
			let newName = prompt("What would you like to be called?", currentName);
			if (!newName)
			{
				return;
			}
			connection.invoke("ChangeName", newName).then(success =>
			{
				if (success)
				{
					writeMessage($('<em></em>').text("Congrats, your new name is " + newName));
					currentName = newName;
					updateCurrentNameDisplay();
				}
				else
				{
					writeMessage(
						$('<em></em>').text("Sorry, unable to change your name"));
				}
				$input.focus();
			});
		});

		$form.submit(function (e) {
			$input.prop('disabled', true);
			connection.send('PostMessage', $input.val()).then(_ =>
			{
				$input.val('').prop('disabled', false).focus();
			});
			return false;
		});

		function updateCurrentNameDisplay()
		{
			$currentName.text("Your Name: " + currentName);
		}

		function writeMessage(messageComponents)
		{
			let message = $('<div></div>');
			for (var i = 0; i < arguments.length; i++)
			{
				message.append(arguments[i]);
			}
			$log.append(message);
			$log.prop('scrollTop', $log.prop('scrollHeight'));
		}
	};
}