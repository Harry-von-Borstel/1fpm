# 1fpm
1fpm -- One File Per Message - File oriented mail system

1fpm means "One file per message" and is an e-mail system that uses normal files to store mails in the way that each mail has its single file to be stored. This makes storage (esp. archiving) easy and transparent. The file format used relies on RFC 822. Furthermore, relying on RFC 822 supports development of message editors that allow full control of the messages sent. This might be useful for sending sophisticated mails like newsletters.

This project is still under construction.

## Roadmap

- Currently, an editor is being developed, that can edit *.eml files. See [EmlEditor](EmlEditor/Readme.md).
- Next, a mail receiver shall download mails from an IMAP server and store them as `*.eml`files locally.
- Finally, a mail sender shall send `*.eml` files thru an SMTP server.

## License
This project is licensed under GPL 3. See [Documentation/License.txt](Documentation/License.txt).
