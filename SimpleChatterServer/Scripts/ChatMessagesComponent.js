function ChatMessagesComponent(userName, $log, $form, $input, chat) {
	chat.client.messagePosted = function (name, message) {
		var nameMessagePair = $('<div></div>')
			.append($('<span></span>').addClass('badge badge-default').text(name))
			.append($('<span></span>').text(' ' + message));
		$log.append(nameMessagePair);
		$log.prop('scrollTop', $log.prop('scrollHeight'));
	};

	this.start = () =>
	{
		$form.submit(function (e) {
			chat.server.postMessage(userName, $input.val());
			$input.val('').focus();
			return false;
		});
	};
}