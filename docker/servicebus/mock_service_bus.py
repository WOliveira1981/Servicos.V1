from http.server import BaseHTTPRequestHandler, HTTPServer
import json

messages = {}


class Handler(BaseHTTPRequestHandler):
    def do_POST(self):
        topic = self.path.strip("/") or "default"
        length = int(self.headers.get("content-length", "0"))
        body = self.rfile.read(length).decode("utf-8")
        messages.setdefault(topic, []).append(body)
        self.send_response(202)
        self.end_headers()
        self.wfile.write(json.dumps({"topic": topic, "accepted": True}).encode("utf-8"))

    def do_GET(self):
        topic = self.path.strip("/") or "default"
        self.send_response(200)
        self.send_header("content-type", "application/json")
        self.end_headers()
        self.wfile.write(json.dumps(messages.get(topic, [])).encode("utf-8"))


HTTPServer(("0.0.0.0", 8080), Handler).serve_forever()
