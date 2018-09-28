/**
 * Sinuous
 * 
 * @author Hakim El Hattab (http://hakim.se, http://twitter.com/hakimel)
 */
var UserProfile =
{
	UA_ANDROID: "android",
	UA_IPHONE: "iphone",
	UA_IPAD: "ipad",
	isOnline: navigator.onLine,
	isAuthenticated: false,
	isTouchDevice: function ()
	{
		return navigator.userAgent.toLowerCase().indexOf(this.UA_ANDROID) != -1 || navigator.userAgent.toLowerCase().indexOf(this.UA_IPHONE) != -1 || navigator.userAgent.toLowerCase().indexOf(this.UA_IPHONE) != -1 ? true : false
	},
	supportsAudio: function ()
	{
		return !this.isTouchDevice()
	},
	supportsAjax: function ()
	{
		return window.XMLHttpRequest != null && this.isOnline
	},
	suuportsLocalStorage: function ()
	{
		return "localStorage" in window && window.localStorage !== null
	}
},
	SinuousSound =
	{
		IDLE: "MusicIdleARR",
		CALM: "MusicCalmARR",
		FUN: "MusicFunARR",
		FX_EXPLOSION: "fx_explosion",
		FX_BREAK: "fx_break",
		FX_BUBBLE: "fx_bubble",
		isMute: false,
		isReady: false,
		initialize: function ()
		{
			var h = document.createElement("div");
			h.setAttribute("id", "sound");
			document.body.appendChild(h);
			UserProfile.supportsAudio() && swfobject.embedSWF("assets/swf/sound.swf", "sound", "1", "1", "9.0.0", "", {
			}, {
				allowScriptAccess: "always"
			}, {
				id: "soundSWF"
			}, this.embedFlashStatusHandler)
		},
		embedFlashStatusHandler: function (h)
		{
			if (h.success)
			{
				SinuousSound.isReady =
				true;
				SinuousSound.isMute ? SinuousSound.mute() : SinuousSound.unmute()
			}
		},
		play: function (h)
		{
			if (UserProfile.supportsAudio() && SinuousSound.isReady) if (document.getElementById("soundSWF") && document.getElementById("soundSWF").style.display != "none") try
			{
				document.getElementById("soundSWF").sendToActionScript(h)
			}
			catch (r)
			{
			}
		},
		mute: function ()
		{
			this.isMute = true;
			if (this.isReady && document.getElementById("soundSWF")) document.getElementById("soundSWF").style.display = "none"
		},
		unmute: function ()
		{
			this.isMute = false;
			if (this.isReady && document.getElementById("soundSWF")) document.getElementById("soundSWF").style.display = "block"
		}
	},
	AJAX =
	{
		post: function (h, r, W)
		{
			$.post(h, r, W)
		}
	};

function Point(h, r)
{
	this.position =
	{
		x: h,
		y: r
	}
}
Point.prototype.distanceTo = function (h)
{
	var r = h.x - this.position.x;
	h = h.y - this.position.y;
	return Math.sqrt(r * r + h * h)
};
Point.prototype.clonePosition = function ()
{
	return {
		x: this.position.x,
		y: this.position.y
	}
};

function Region()
{
	this.top = this.left = Number.POSITIVE_INFINITY;
	this.bottom = this.right = 0
}
Region.prototype.reset = function ()
{
	this.top = this.left = Number.POSITIVE_INFINITY;
	this.bottom = this.right = 0
};
Region.prototype.inflate = function (h, r)
{
	this.left = Math.min(this.left, h);
	this.top = Math.min(this.top, r);
	this.right = Math.max(this.right, h);
	this.bottom = Math.max(this.bottom, r)
};
var SinuousWorld = new

function ()
{
	function h()
	{
		if (UserProfile.suuportsLocalStorage())
		{
			localStorage.unlockedLevels = m.unlockedLevels;
			localStorage.selectedLevel = m.selectedLevel;
			localStorage.mute = m.mute
		}
	}

	function r()
	{
		if (m.mute)
		{
			K.innerHTML = Ba;
			SinuousSound.mute()
		}
		else
		{
			K.innerHTML = Ca;
			SinuousSound.unmute()
		}
	}

	function W()
	{
		m.mute = !m.mute;
		r();
		h();
		event.preventDefault()
	}

	function Da()
	{
		if (UserProfile.suuportsLocalStorage())
		{
			localStorage.unlockedLevels = null;
			localStorage.selectedLevel = null;
			m.unlockedLevels = 1;
			s = m.selectedLevel =
			1
		}
		L();
		event.preventDefault();
		alert("Game history was reset.")
	}

	function Ea()
	{
		t || B.setAttribute("class", "open")
	}

	function Fa()
	{
		B.setAttribute("class", "")
	}

	function Ga(b)
	{
		if (t == false)
		{
			SinuousSound.play(SinuousSound.CALM);
			t = true;
			v = [];
			y = [];
			G = o = 0;
			q.fc = 0;
			q.fs = 0;
			q.ms = 0;
			q.cs = 0;
			s = m.selectedLevel;
			a.trail = [];
			a.position.x = C;
			a.position.y = D;
			a.shield = 0;
			a.gravity = 0;
			a.flicker = 0;
			a.lives = ha;
			a.timewarped = false;
			a.timefactor = 0;
			a.sizewarped = false;
			a.sizefactor = 0;
			a.gravitywarped = false;
			a.gravityfactor = 0;
			x.style.display = "none";
			w.style.display = "block";
			H.style.display = "none";
			M = (new Date).getTime()
		}
		UserProfile.isTouchDevice() || b.preventDefault()
	}

	function ia()
	{
		SinuousSound.play(SinuousSound.IDLE);
		SinuousSound.play(SinuousSound.FX_EXPLOSION);
		t = false;
		ja = (new Date).getTime() - M;
		Ha();
		x.style.display = "block";
		o = Math.round(o);
		ka.innerHTML = "Game Over! (" + o + " points)";
		scoreText = "Level: <span>" + s + "</span>";
		scoreText += " Score: <span>" + Math.round(o) + "</span>";
		scoreText += " Time: <span>" + Math.round(((new Date).getTime() - M) / 1E3 * 100) / 100 + "s</span>";
		w.innerHTML = scoreText
	}

	function L()
	{
		for (var b = N.getElementsByTagName("li"), d = 0, c = b.length; d < c; d++)
		{
			var g = d >= m.unlockedLevels ? "locked" : "unlocked";
			if (d + 1 == m.selectedLevel) g = "selected";
			b[d].setAttribute("class", g)
		}
	}

	function Ia(b)
	{
		if (b.target.getAttribute("class") == "unlocked")
		{
			m.selectedLevel = parseInt(b.target.getAttribute("data-level"));
			s = m.selectedLevel;
			L();
			h()
		}
		b.preventDefault()
	}

	function O()
	{
		la.style.display = "none";
		H.style.display = "none";
		UserProfile.isOnline = false
	}

	function Ja()
	{
		for (var b = k.length < 10, d = 0; d < k.length; d++) if (o > k[d].score)
		{
			b = true;
			break
		}
		if (b) if (!P.value || P.value == " ") alert("Name can not be empty.");
		else
		{
			Ka();
			H.style.display = "none"
		}
	}

	function Ha()
	{
		AJAX.post("/highscore", "m=ghs" + (z ? "&table=facebook" : ""), function (b)
		{
			if ((k = eval(b)) && t == false && UserProfile.isAuthenticated)
			{
				b = 1;
				for (var d = 0; d < k.length; d++) k[d].score > o && b++;
				if (b < 10)
				{
					if (k.length > 1) if (d = k.length >= 9 ? k.pop() : {
					})
					{
						d.name = "";
						d.score = Math.round(o);
						d.date = "";
						newHighscoreData = k.slice(0, b - 1);
						newHighscoreData.push(d);
						k = newHighscoreData =
						newHighscoreData.concat(k.slice(b - 1));
						X()
					}
					ma.innerHTML = "You made #" + b + " on the top list!";
					H.style.display = "block"
				}
			}
		}, function ()
		{
			O()
		})
	}

	function La()
	{
		AJAX.post("/highscore", "m=ghs" + (z ? "&table=facebook" : ""), function (b)
		{
			k = eval(b);
			X()
		}, function ()
		{
			O()
		})
	}

	function Ka()
	{
		var b = P.value,
			d = Math.round(ja / 1E3 * 100) / 100,
			c = o * o * 3.14159265 * Math.max(b.length, 1),
			g = "m=shs";
		g += "&n=" + b;
		g += "&s=" + c;
		g += "&d=" + d;
		g += "&sc=" + sc;
		g += "&fc=" + Math.round(q.fc);
		g += "&fs=" + Math.round(q.fs);
		g += "&ms=" + Math.round(q.ms);
		g += "&cs=" + Math.round(q.cs);
		g += "&f=" + Math.round((Y + Z + A) / 3);
		g += z ? "&table=facebook" : "";
		AJAX.post("/highscore", g, function (i)
		{
			k = eval(i);
			X()
		}, function ()
		{
			O()
		})
	}

	function X()
	{
		if (k)
		{
			for (var b = "", d = 0; d < k.length; d++)
			{
				b += "<li>";
				b += '<span class="place">' + (d + 1) + ".</span>";
				b += '<span class="name">' + k[d].name + "</span>";
				b += '<span class="score">' + k[d].score + " p</span>";
				b += '<span class="date">' + k[d].date + "</span>";
				b += "</li>"
			}
			na.innerHTML = b
		}
	}

	function Ma(b)
	{
		if (z)
		{
			C = b.clientX - $(u).offset().left;
			D = b.clientY - $(u).offset().top
		}
		else
		{
			C = b.clientX - (window.innerWidth - j.width) * 0.5 - 6;
			D = b.clientY - (window.innerHeight - j.height) * 0.5 - 6
		}
	}

	function Na()
	{
	}

	function Oa()
	{
	}

	function Pa(b)
	{
		if (b.touches.length == 1)
		{
			b.preventDefault();
			C = b.touches[0].pageX - (window.innerWidth - j.width) * 0.5;
			D = b.touches[0].pageY - (window.innerHeight - j.height) * 0.5
		}
	}

	function Qa(b)
	{
		if (b.touches.length == 1)
		{
			b.preventDefault();
			C = b.touches[0].pageX - (window.innerWidth - j.width) * 0.5 - 60;
			D = b.touches[0].pageY - (window.innerHeight - j.height) * 0.5 - 30
		}
	}

	function Ra()
	{
	}

	function oa()
	{
		j.width = UserProfile.isTouchDevice() ? window.innerWidth : pa;
		j.height = UserProfile.isTouchDevice() ? window.innerHeight : qa;
		u.width = j.width;
		u.height = j.height;
		var b = z ? 0 : 6;
		if (UserProfile.isTouchDevice())
		{
			x.style.left = "0px";
			x.style.top = "0px";
			w.style.left = "0px";
			w.style.top = "0px"
		}
		else
		{
			x.style.left = b + "px";
			x.style.top = Math.round(j.height / 4) + "px";
			w.style.left = b + "px";
			w.style.top = b + "px"
		}
	}

	function I(b, d)
	{
		for (var c = 10 + Math.random() * 15; --c >= 0;)
		{
			var g = new Point;
			g.position.x = b.x + Math.sin(c) * d;
			g.position.y = b.y + Math.cos(c) * d;
			g.velocity =
			{
				x: -4 + Math.random() * 8,
				y: -4 + Math.random() * 8
			};
			g.alpha = 1;
			Q.push(g)
		}
	}

	function ra()
	{
		var b = (new Date).getTime();
		aa++;
		if (b > ba + 1E3)
		{
			A = Math.min(Math.round(aa * 1E3 / (b - ba)), E);
			Y = Math.min(Y, A);
			Z = Math.max(Z, A);
			ba = b;
			aa = 0
		}
		f.clearRect(0, 0, u.width, u.height);
		var d = F[s - 1],
			c = F[s];
		b = d.factor;
		var g = d.multiplier;
		if (s < F.length && t) b += G / d.duration * (c.factor - d.factor);
		d = 0.01 + Math.max(Math.min(A, E), 0) / E * 0.99;
		(d = d * d * g) || (d = 0.5);
		g =
		{
			x: R.x * b * (1 - a.timefactor),
			y: R.y * b * (1 - a.timefactor)
		};
		var i, l;
		i = a.flicker % 4 == 1 || a.flicker % 4 == 2;
		if (t)
		{
			pp = a.clonePosition();
			a.position.x += (C - a.position.x) * sa;
			a.position.y += (D - a.position.y) * sa;
			o += 0.4 * b * d;
			o += a.distanceTo(pp) * 0.1 * d;
			q.fc++;
			q.fs += 0.4 * b * d;
			q.ms += a.distanceTo(pp) * 0.1 * d;
			a.flicker = Math.max(a.flicker - 1, 0);
			a.shield = Math.max(a.shield - 1, 0);
			a.gravity = Math.max(a.gravity - 0.35, 0);
			if (a.timewarped)
			{
				if (a.timefactor > 0.5999) a.timewarped = false;
				a.timefactor += (0.6 - a.timefactor) * 0.1
			}
			else a.timefactor += (0 - a.timefactor) * 0.002;
			a.timefactor = Math.max(Math.min(a.timefactor, 1), 0);
			if (a.sizewarped)
			{
				if (a.sizefactor > 0.5999) a.sizewarped = false;
				a.sizefactor += (0.6 - a.sizefactor) * 0.04
			}
			else a.sizefactor += (0 - a.sizefactor) * 0.01;
			a.sizefactor = Math.max(Math.min(a.sizefactor, 1), 0);
			if (a.gravitywarped)
			{
				if (a.gravityfactor > 0.99995) a.gravitywarped = false;
				a.gravityfactor += (1 - a.gravityfactor) * 0.04
			}
			else
			{
				if (a.gravityfactor < 0.12) a.gravityfactor = 0;
				a.gravityfactor += (0 - a.gravityfactor) * 0.014
			}
			a.gravityfactor = Math.max(Math.min(a.gravityfactor, 1), 0);
			if (a.shield > 0 && (a.shield > 100 || a.shield % 3 != 0))
			{
				f.beginPath();
				f.fillStyle = "#167a66";
				f.strokeStyle = "#00ffcc";
				f.arc(a.position.x, a.position.y, a.size * (Math.min(a.shield, 100) / 50), 0, Math.PI * 2, true);
				f.fill();
				f.stroke()
			}
			if (a.gravityfactor > 0)
			{
				l = a.gravityfactor * ta;
				c = f.createRadialGradient(a.position.x, a.position.y, 0, a.position.x, a.position.y, l);
				c.addColorStop(0.1, "rgba(0, 70, 70, 0.8)");
				c.addColorStop(0.8, "rgba(0, 70, 70, 0)");
				f.beginPath();
				f.fillStyle = c;
				f.arc(a.position.x, a.position.y, l, 0, Math.PI * 2, true);
				f.fill()
			}
			for (; a.trail.length - 1 < 60;) a.trail.push(new Point(a.position.x, a.position.y));
			f.beginPath();
			f.strokeStyle = i ? "333333" : "#648d93";
			f.lineWidth = 2;
			c = 0;
			for (l = a.trail.length; c < l; c++)
			{
				p = a.trail[c];
				p2 = a.trail[c + 1];
				if (c == 0) f.moveTo(p.position.x, p.position.y);
				else p2 && f.quadraticCurveTo(p.position.x, p.position.y, p.position.x + (p2.position.x - p.position.x) / 2, p.position.y + (p2.position.y - p.position.y) / 2);
				p.position.x += g.x;
				p.position.y += g.y
			}
			f.stroke();
			f.closePath();
			l = 0;
			for (c = a.trail.length - 1; c > 0; c--)
			{
				p = a.trail[c];
				if (c == Math.round(51) || c == Math.round(45) || c == Math.round(39))
				{
					f.beginPath();
					f.lineWidth = 0.5;
					f.fillStyle = i ? "#333333" : "#8ff1ff";
					f.arc(p.position.x, p.position.y, 2.5, 0, Math.PI * 2, true);
					f.fill();
					l++
				}
				if (l == a.lives) break
			}
			a.trail.length > 60 && a.trail.shift();
			f.beginPath();
			f.fillStyle = i ? "#333333" : "#8ff1ff";
			f.arc(a.position.x, a.position.y, a.size / 2, 0, Math.PI * 2, true);
			f.fill()
		}
		if (t && (a.position.x < 0 || a.position.x > j.width || a.position.y < 0 || a.position.y > j.height))
		{
			I(a.position, 10);
			ia()
		}
		for (c = 0; c < v.length; c++)
		{
			p = v[c];
			p.size = p.originalSize * (1 - a.sizefactor);
			p.offset.x *= 0.95;
			p.offset.y *= 0.95;
			i = p.distanceTo(a.position);
			if (t) if (a.gravityfactor > 0)
			{
				var ua = Math.atan2(p.position.y - a.position.y, p.position.x - a.position.x);
				l = a.gravityfactor * ta;
				if (i < l)
				{
					p.offset.x += (Math.cos(ua) * (l - i) - p.offset.x) * 0.2;
					p.offset.y += (Math.sin(ua) * (l - i) - p.offset.y) * 0.2
				}
			}
			else if (a.shield > 0 && i < (a.size * 4 + p.size) * 0.5)
			{
				SinuousSound.play(SinuousSound.FX_BREAK);
				I(p.position, 10);
				v.splice(c, 1);
				c--;
				o += 20 * d;
				q.cs += 20 * d;
				S(Math.ceil(20 * d), p.clonePosition(), p.force);
				continue
			}
			else if (i < (a.size + p.size) * 0.5 && a.flicker == 0) if (a.lives > 0)
			{
				I(a.position, 4);
				a.lives--;
				a.flicker += 60;
				v.splice(c, 1);
				c--
			}
			else
			{
				I(a.position, 10);
				ia()
			}
			f.beginPath();
			f.fillStyle = "#ff0000";
			f.arc(p.position.x + p.offset.x, p.position.y + p.offset.y, p.size / 2, 0, Math.PI * 2, true);
			f.fill();
			p.position.x += g.x * p.force;
			p.position.y += g.y * p.force;
			if (p.position.x < -p.size || p.position.y > j.height + p.size)
			{
				v.splice(c, 1);
				c--;
				t && G++
			}
		}
		for (c = 0; c < y.length; c++)
		{
			p = y[c];
			if (p.distanceTo(a.position) < (a.size + p.size) * 0.5 && t)
			{
				SinuousSound.play(SinuousSound.FX_BUBBLE);
				if (p.type == T)
				{
					SinuousSound.play(SinuousSound.FUN);
					a.shield = 300
				}
				else if (p.type == J)
				{
					if (a.lives < ca)
					{
						S("LIFE UP!", p.clonePosition(), p.force);
						a.lives = Math.min(a.lives + 1, ca)
					}
				}
				else if (p.type == U) a.gravitywarped = true;
				else if (p.type == da) a.timewarped = true;
				else if (p.type == ea) a.sizewarped = true;
				if (p.type != J)
				{
					o += 50 * d;
					q.cs += 50 * d;
					S(Math.ceil(50 * d), p.clonePosition(), p.force)
				}
				for (i = 0; i < v.length; i++)
				{
					e = v[i];
					if (e.distanceTo(p.position) < 100)
					{
						SinuousSound.play(SinuousSound.FX_BREAK);
						I(e.position, 10);
						v.splice(i, 1);
						i--;
						o += 20 * d;
						q.cs += 20 * d;
						S(Math.ceil(20 * d), e.clonePosition(), e.force)
					}
				}
				y.splice(c, 1);
				c--
			}
			else if (p.position.x < -p.size || p.position.y > j.height + p.size)
			{
				y.splice(c, 1);
				c--
			}
			i = "";
			l = "#000";
			if (p.type === T)
			{
				i = "S";
				l = "#007766"
			}
			else if (p.type === J)
			{
				i = "1";
				l = "#009955"
			}
			else if (p.type === U)
			{
				i = "G";
				l = "#225599"
			}
			else if (p.type === da)
			{
				i = "T";
				l = "#665599"
			}
			else if (p.type === ea)
			{
				i = "M";
				l = "#acac00"
			}
			f.beginPath();
			f.fillStyle = l;
			f.arc(p.position.x, p.position.y, p.size / 2, 0, Math.PI * 2, true);
			f.fill();
			f.save();
			f.font = "bold 12px Arial";
			f.fillStyle = "#ffffff";
			f.fillText(i, p.position.x - f.measureText(i).width * 0.5, p.position.y + 4);
			f.restore();
			p.position.x += g.x * p.force;
			p.position.y += g.y * p.force
		}
		v.length < 27 * b && v.push(va(new wa));
		if (y.length < 1 && Math.random() > 0.994 && a.isBoosted() == false)
		{
			for (b = new fa; b.type == J && a.lives >= ca;) b.randomizeType();
			y.push(va(b))
		}
		a.shield == 1 && t && SinuousSound.play(SinuousSound.CALM);
		for (c = 0; c < Q.length; c++)
		{
			p = Q[c];
			p.velocity.x += (g.x - p.velocity.x) * 0.04;
			p.velocity.y += (g.y - p.velocity.y) * 0.04;
			p.position.x += p.velocity.x;
			p.position.y += p.velocity.y;
			p.alpha -= 0.02;
			f.fillStyle = "rgba(255,255,255," + Math.max(p.alpha, 0) + ")";
			f.fillRect(p.position.x, p.position.y, 1, 1);
			p.alpha <= 0 && Q.splice(c, 1)
		}
		for (c = 0; c < V.length; c++)
		{
			p = V[c];
			p.position.x += g.x * p.force;
			p.position.y += g.y * p.force;
			p.position.y -= 1;
			f.save();
			f.font = "10px Arial";
			f.fillStyle = "rgba( 255, 255, 255, " + p.alpha + " )";
			f.fillText(p.text, p.position.x - f.measureText(p.text).width * 0.5, p.position.y);
			f.restore();
			p.alpha *= 0.96;
			if (p.alpha < 0.05)
			{
				V.splice(c, 1);
				c--
			}
		}
		if (n.message && n.message !== "")
		{
			n.progress += (n.target - n.progress) * 0.05;
			if (n.progress > 0.9999999) n.target = 0;
			else if (n.target == 0 && n.progress < 0.05) n.message = "";
			f.save();
			f.font = "bold 22px Arial";
			p =
			{
				x: j.width - f.measureText(n.message).width - 15,
				y: j.height + 40 - 55 * n.progress
			};
			f.translate(p.x, p.y);
			f.fillStyle = "rgba( 0, 0, 0, " + n.progress * 0.4 + " )";
			f.fillRect(-15, -30, 200, 100);
			f.fillStyle = "rgba( 255, 255, 255, " + n.progress + " )";
			f.fillText(n.message, 0, 0);
			f.restore()
		}
		if (t)
		{
			if (G > F[s - 1].duration)
			{
				if (s < F.length)
				{
					s++;
					G = 0;
					m.unlockedLevels = Math.max(m.unlockedLevels, s);
					h();
					L();
					b = true
				}
				else b = false;
				if (b)
				{
					n.message = "LEVEL " + s + "!";
					n.progress = 0;
					n.target = 1
				}
			}
			scoreText = "Level: <span>" + s + "</span>";
			scoreText += " Score: <span>" + Math.round(o) + "</span>";
			scoreText += " Time: <span>" + Math.round(((new Date).getTime() - M) / 1E3 * 100) / 100 + "s</span>";
			scoreText += ' <p class="fps">FPS: <span>' + Math.round(A) + " (" + Math.round(Math.max(Math.min(A / E, E), 0) * 100) + "%)</span></p>";
			w.innerHTML = scoreText
		}
	}

	function S(b, d, c)
	{
		V.push(
		{
			text: b,
			position: {
				x: d.x,
				y: d.y
			},
			alpha: 1,
			force: c
		})
	}

	function va(b)
	{
		if (Math.random() > 0.5)
		{
			b.position.x = Math.random() * j.width;
			b.position.y = -20
		}
		else
		{
			b.position.x = j.width + 20;
			b.position.y = -j.height * 0.2 + Math.random() * j.height * 1.2
		}
		return b
	}

	function ga()
	{
		this.position =
		{
			x: 0,
			y: 0
		};
		this.trail = [];
		this.size = 8;
		this.shield = 0;
		this.lives = ha;
		this.flicker = 0;
		this.gravitywarped = false;
		this.gravityfactor = 0;
		this.timewarped = false;
		this.timefactor = 0;
		this.sizewarped = false;
		this.sizefactor = 0
	}

	function wa()
	{
		this.position =
		{
			x: 0,
			y: 0
		};
		this.offset =
		{
			x: 0,
			y: 0
		};
		this.originalSize = this.size = 6 + Math.random() * 4;
		this.force = 1 + Math.random() * 0.4
	}

	function fa()
	{
		this.type = null;
		this.position =
		{
			x: 0,
			y: 0
		};
		this.size = 20 + Math.random() * 4;
		this.force = 0.8 + Math.random() * 0.4;
		this.randomizeType()
	}
	var z = window.FACEBOOK_MODE || false,
		pa = z ? 758 : 1E3,
		qa = z ? 500 : 600,
		E = 60,
		sa = 0.25,
		ha = 2,
		ca = 3,
		ta = 120,
		Ba = "Turn audio on",
		Ca = "Turn audio off",
		T = "shield",
		J = "life",
		U = "gravitywarp",
		da = "timewarp",
		ea = "sizewarp",
		xa = [T, T, J, U, U, da, ea],
		j =
		{
			x: 0,
			y: 0,
			width: UserProfile.isTouchDevice() ? window.innerWidth : pa,
			height: UserProfile.isTouchDevice() ? window.innerHeight : qa
		},
		u, f, B, w, x, ka, N, ya, K, za = null,
		n =
		{
			messsage: "",
			progress: 0,
			target: 0
		},
		v = [],
		y = [],
		Q = [],
		V = [],
		a = null,
		C = window.innerWidth - j.width,
		D = window.innerHeight - j.height,
		t = false,
		o = 0,
		M =
		0,
		ja = 0,
		G = 0,
		s = 1,
		F = [
		{
			factor: 1.2,
			duration: 100,
			multiplier: 0.5
		}, {
			factor: 1.4,
			duration: 200,
			multiplier: 0.6
		}, {
			factor: 1.6,
			duration: 300,
			multiplier: 0.7
		}, {
			factor: 1.8,
			duration: 450,
			multiplier: 0.8
		}, {
			factor: 2,
			duration: 600,
			multiplier: 1
		}, {
			factor: 2.4,
			duration: 800,
			multiplier: 1.1
		}, {
			factor: 2.9,
			duration: 1E3,
			multiplier: 1.3
		}, {
			factor: 3.5,
			duration: 1300,
			multiplier: 1.7
		}, {
			factor: 4.8,
			duration: 2E3,
			multiplier: 2
		}],
		m =
		{
			unlockedLevels: 1,
			selectedLevel: 1,
			mute: false
		},
		R =
		{
			x: -1.3,
			y: 1
		},
		q =
		{
			fc: 0,
			fs: 0,
			ms: 0,
			cs: 0
		},
		A =
		{
			fps: 0,
			fpsMin: 1E3,
			fpsMax: 0
		},
		Y =
		1E3,
		Z = 0,
		ba = (new Date).getTime(),
		aa = 0,
		k = [],
		la, na, H, P, Aa, ma = null;
	this.initialize = function ()
	{
		u = document.getElementById("world");
		B = document.getElementsByTagName("header")[0];
		x = document.getElementById("game-panels");
		w = document.getElementById("game-status");
		document.getElementById("message");
		ka = document.getElementById("title");
		N = document.getElementById("level-selector");
		ya = document.getElementById("start-button");
		K = document.getElementById("mute-button");
		za = document.getElementById("reset-button");
		la = document.getElementById("highscore-list");
		na = document.getElementById("highscore-output");
		H = document.getElementById("highscore-win");
		P = document.getElementById("highscore-input");
		Aa = document.getElementById("highscore-submit");
		ma = document.getElementById("highscore-place");
		if (u && u.getContext)
		{
			f = u.getContext("2d");
			document.addEventListener("mousemove", Ma, false);
			document.addEventListener("mousedown", Na, false);
			document.addEventListener("mouseup", Oa, false);
			u.addEventListener("touchstart", Pa, false);
			document.addEventListener("touchmove", Qa, false);
			document.addEventListener("touchend", Ra, false);
			ya.addEventListener("click", Ga, false);
			K.addEventListener("click", W, false);
			za.addEventListener("click", Da, false);
			Aa.addEventListener("click", Ja, false);
			B.addEventListener("mouseover", Ea, false);
			B.addEventListener("mouseout", Fa, false);
			window.addEventListener("resize", oa, false);
			SinuousSound.initialize();
			if (UserProfile.suuportsLocalStorage())
			{
				var b = parseInt(localStorage.unlockedLevels),
					d = parseInt(localStorage.selectedLevel),
					c = localStorage.mute;
				if (b) m.unlockedLevels =
				b;
				if (d) m.selectedLevel = d;
				if (c) m.mute = c == "true"
			}
			r();
			c = "";
			b = 1;
			for (d = F.length; b <= d; b++) c += '<li data-level="' + b + '">' + b + "</li>";
			N.getElementsByTagName("ul")[0].innerHTML = c;
			c = N.getElementsByTagName("li");
			b = 0;
			for (d = c.length; b < d; b++) c[b].addEventListener("click", Ia, false);
			L();
			a = new ga;
			oa();
			if (UserProfile.isTouchDevice())
			{
				w.style.width = j.width + "px";
				u.style.border = "none";
				B.style.display = "none";
				R.x *= 2;
				R.y *= 2;
				setInterval(ra, 1E3 / 30)
			}
			else setInterval(ra, 1E3 / E);
			UserProfile.isOnline || O();
			u.style.display = "block";
			x.style.display = "block";
			La()
		}
	};
	ga.prototype = new Point;
	ga.prototype.isBoosted = function ()
	{
		return this.shield != 0 || this.gravityfactor != 0
	};
	wa.prototype = new Point;
	fa.prototype = new Point;
	fa.prototype.randomizeType = function ()
	{
		this.type = xa[Math.round(Math.random() * (xa.length - 1))]
	}
};
if (FACEBOOK_MODE)
{
	UserProfile.isOnline = true;
	UserProfile.isAuthenticated = true;
	SinuousWorld.initialize()
}
else AJAX.post("/login-verify", "", function (h)
{
	UserProfile.isOnline = true;
	if (h == "false")
	{
		UserProfile.isAuthenticated = false;
		document.getElementById("highscore-list").innerHTML += '<p class="auth out">You need to <a href="/login">sign in</a> with a Google account to be eligible for the leaderboard.</p>'
	}
	else
	{
		UserProfile.isAuthenticated = true;
		document.getElementById("highscore-list").innerHTML += '<p class="auth in">Logged in as ' + h + '. <a href="/logout">Sign out?</a></p>'
	}
	SinuousWorld.initialize()
}, function ()
{
	UserProfile.isOnline = false;
	UserProfile.isAuthenticated = false;
	SinuousWorld.initialize()
});

function sendToJavaScript(h)
{
	h == "SoundController ready and loaded!" && SinuousSound.play(SinuousSound.IDLE)
};