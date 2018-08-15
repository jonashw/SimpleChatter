function ChatMessagesComponent(userName, $log, $form, $input) {
	this.start = connection =>
	{
		connection.on('MessagePosted', (name, message) => {
			var nameMessagePair = $('<div></div>')
				.append($('<strong style="margin-right:4px;"></strong>').text(name))
				.append($('<span></span>').text(' ' + message));
			$log.append(nameMessagePair);
			$log.prop('scrollTop', $log.prop('scrollHeight'));
		});

		$form.submit(function (e) {
			$input.prop('disabled', true);
			connection.send('PostMessage', userName, $input.val()).then(_ =>
			{
				$input.val('').prop('disabled', false).focus();
			});
			return false;
		});
	};
}