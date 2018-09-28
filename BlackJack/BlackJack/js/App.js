$(document).ready(function ()
{
	window.App = new App();
});
var App = function ()
{
	this.initialize.apply(this, arguments);
};
App.prototype = (function ()
{
	var S = {};
	var z = 300,
		O = 32,
		P = 83,
		ac = 68,
		aq = 49,
		ao = 50,
		am = 51,
		r = [
			[{
				deg: 0,
				top: 0
			}],
			[{
				deg: 5,
				top: 0
			}, {
				deg: -5,
				top: 0
			}],
			[{
				deg: 5,
				top: 15
			}, {
				deg: -1,
				top: 0
			}, {
				deg: -5,
				top: 15
			}],
			[{
				deg: 9,
				top: 20
			}, {
				deg: 4,
				top: 0
			}, {
				deg: -4,
				top: 0
			}, {
				deg: -9,
				top: 15
			}],
			[{
				deg: 12,
				top: 50
			}, {
				deg: 8,
				top: 10
			}, {
				deg: -4,
				top: 0
			}, {
				deg: -12,
				top: 15
			}, {
				deg: -16,
				top: 40
			}],
			[{
				deg: 14,
				top: 40
			}, {
				deg: 8,
				top: 10
			}, {
				deg: -2,
				top: 5
			}, {
				deg: -5,
				top: 15
			}, {
				deg: -8,
				top: 40
			}, {
				deg: -14,
				top: 70
			}],
			[{
				deg: 14,
				top: 70
			}, {
				deg: 8,
				top: 30
			}, {
				deg: 4,
				top: 10
			}, {
				deg: 0,
				top: 5
			}, {
				deg: -4,
				top: 20
			}, {
				deg: -8,
				top: 40
			}, {
				deg: -16,
				top: 70
			}]
		];
	var ae = ["clubs", "diamonds", "hearts", "spades"],
		W = [],
		ad = 0,
		T = false,
		q = false,
		o = $("#deal"),
		g = $("#actions"),
		y = $("#double"),
		v = $("#player-cards"),
		a = $("#dealer-cards"),
		M = $("#player-total"),
		U = [],
		i = 0,
		ai = $("#dealer-total"),
		al = [],
		Q = 0,
		V = $("#chips"),
		b = $(".chip"),
		ap = 100,
		ar = $("#bankroll"),
		n = false,
		A = b.first().data("value"),
		Z = null,
		w = true,
		C = false,
		G = false,
		F = $("html");
	S.initialize = function (au)
	{
		h();
	};
	S.deal = function ()
	{
		I();
	};
	S.hit = function ()
	{
		D();
	};
	S.stand = function ()
	{
		an();
	};
	S.doubledown = function ()
	{
		aa();
	};
	var h = function ()
	{
		$('a[href="#"]').bind("click", function (au)
		{
			au.preventDefault();
		});
		e();
		c();
		p();
		setTimeout(function ()
		{
			window.scrollTo(0, 1);
		}, 500);
	};
	var c = function ()
	{
		$(window).bind("resize", f);
		f(null);
	};
	var f = function (au)
	{
		clearTimeout(Z);
		Z = setTimeout(function ()
		{
			j();
		}, 100);
	};
	var p = function ()
	{
		$(document).bind("keydown", l);
		$(document).bind("keyup", d);
	};
	var l = function (au)
	{
		switch (au.keyCode)
		{
			case O:
				(T) ? g.children("li:first-child").children("a").addClass("active") : o.children("a").addClass("active");
				break;
			case P:
				g.children("li:nth-child(2)").children("a").addClass("active");
				break;
			case ac:
				g.children("li:nth-child(3)").children("a").addClass("active");
				break;
			case aq:
				at(0);
				break;
			case ao:
				at(1);
				break;
			case am:
				at(2);
				break;
		}
	};
	var d = function (au)
	{
		au.preventDefault();
		switch (au.keyCode)
		{
			case O:
				if (T)
				{
					D();
					g.children("li:first-child").children("a").removeClass("active");
				} else
				{
					I();
					o.children("a").removeClass("active");
				}
			case P:
				an();
				g.children("li:nth-child(2)").children("a").removeClass("active");
				break;
			case ac:
				aa();
				g.children("li:nth-child(3)").children("a").removeClass("active");
				break;
			case aq:
				at(0);
				break;
			case ao:
				at(1);
				break;
			case am:
				at(2);
				break;
		}
	};
	var at = function (au)
	{
		if (T || G)
		{
			return;
		}
		b.eq(au).trigger("click");
	};
	var Y = function ()
	{
		for (var av = 0; av < ae.length; av++)
		{
			for (var au = 1; au <= 13; au++)
			{
				var aw = (au > 10) ? 10 : au;
				W.push({
					card: au,
					value: aw,
					type: ae[av]
				});
			}
		}
		W.shuffle();
	};
	var E = function (ay, ax, aA)
	{
		var av = W[ad],
			au = (ax == "player") ? v : a,
			aw = J(ad, av.type, av.card, ay),
			az = 0;
		ad++;
		w = false;
		aw.css({
			top: "-150%",
			left: "100%"
		});
		au.append(aw);
		az = (ax == "player") ? aw.index() : 50 - aw.index();
		aw.css("z-index", az);
		setTimeout(function ()
		{
			aw.css({
				top: "0%",
				left: 10 * aw.index() + "%"
			});
			X(au, (ax == "player"));
			setTimeout(function ()
			{
				R(au);
				if (ax == "player")
				{
					L(av.value);
				} else
				{
					ah(av.value);
				}
				w = true;
				if (aA != undefined)
				{
					aA.call();
				}
			}, z + 100);
		}, 10);
	};
	var X = function (aw, av)
	{
		var ay = aw.children(".card"),
			az = ay.size() - 1,
			au = (av) ? -1 : 1,
			ax = (r[az]) ? r[az] : r[r.length - 1];
		ay.each(function (aA)
		{
			var aB = (aA < ax.length) ? ax[aA].deg : ax[ax.length - 1].deg,
				aC = (aA < ax.length) ? ax[aA].top : ax[ax.length - 1].top + (20 * (aA - ax.length + 1));
			$(this).css({
				"-webkit-transform": "rotate(" + aB * au + "deg)",
				"-khtml-transform": "rotate(" + aB * au + "deg)",
				"-moz-transform": "rotate(" + aB * au + "deg)",
				"-ms-transform": "rotate(" + aB * au + "deg)",
				transform: "rotate(" + aB * au + "deg)",
				top: aC * -au + "px"
			});
		});
	};
	var j = function ()
	{
		R(v);
		R(a);
	};
	var R = function (av)
	{
		var aw = av.children(".card:last-child"),
			au = 0;
		if (aw.size() == 0)
		{
			return;
		}
		au = aw.position().left + aw.width();
		if (F.attr("browser") == "Safari")
		{
			av.css("-webkit-transform", "translate3d(" + -au / 2 + "px,0,0)");
		} else
		{
			av.css("margin-left", -au / 2 + "px");
		}
	};
	var J = function (av, aB, aC, aA)
	{
		var aw;
		if (aA == "back")
		{
			aw = $('<div data-id="' + av + '" class="card back"></div>');
		} else
		{
			var az = (aC == 1) ? "A" : (aC == 11) ? "J" : (aC == 12) ? "Q" : (aC == 13) ? "K" : aC,
				au = (aB == "hearts") ? "♥" : (aB == "diamonds") ? "♦" : (aB == "spades") ? "♠" : "♣",
				aD = "<div><span>" + az + "</span><span>" + au + "</span></div>",
				aE = "";
			if (aC <= 10)
			{
				for (var ay = 1, ax = aC; ay <= ax; ay++)
				{
					aE += "<span>" + au + "</span>";
				}
			} else
			{
				aE = (aC == 11) ? "<span>♝</span>" : (aC == 12) ? "<span>♛</span>" : (aC == 13) ? "<span>♚</span>" : "";
			}
			aw = $('<div data-id="' + av + '" class="card value' + az + " " + aB + '">' + aD + '<div class="icons">' + aE + "</div>" + aD + "</div>");
		}
		return aw;
	};
	var I = function ()
	{
		if (T || !w || G)
		{
			return;
		}
		T = true;
		if (q)
		{
			y.removeClass("desactivate");
			M.html("");
			ai.html("");
			i = 0;
			Q = 0;
			U = [];
			al = [];
			W = [];
			ad = 0;
			n = false;
			w = true;
			C = false;
			$("#message").remove();
		}
		v.html("");
		a.html("");
		Y();
		s(-1);
		x();
		q = true;
	};
	var D = function ()
	{
		if (!T || !w || C || G)
		{
			return;
		}
		y.addClass("desactivate");
		E("front", "player", function ()
		{
			if (U.sum() > 21)
			{
				aj("lose-busted");
			}
		});
	};
	var an = function ()
	{
		if (!T || !w || C || G)
		{
			return;
		}
		C = true;
		K();
		setTimeout(function ()
		{
			if (al.sum() < 17)
			{
				k();
			} else
			{
				u();
			}
		}, z);
	};
	var k = function ()
	{
		E("front", "dealer", function ()
		{
			ai.html(N());
			if (al.sum() < 17)
			{
				k();
			} else
			{
				u();
			}
		});
	};
	var aa = function ()
	{
		if (!T || !w || C || y.hasClass("desactivate") || G)
		{
			return;
		}
		s(-1);
		n = true;
		E("front", "player", function ()
		{
			if (U.sum() > 21)
			{
				aj("lose-busted");
			} else
			{
				an();
			}
		});
	};
	var ag = function (av)
	{
		H(av);
		var au = (n) ? 2 : 1;
		s(au);
		m();
	};
	var af = function (av)
	{
		H(av);
		var au = (n) ? 4 : 2;
		s(au);
		m();
	};
	var aj = function (au)
	{
		H(au);
		s(0);
		m();
	};
	var H = function (au)
	{
		var ax = document.createElement("div"),
			aw = "",
			av = $("#message");
		if (av.size() > 0)
		{
			av.remove();
		}
		ax.className = au;
		ax.id = "message";
		switch (au)
		{
			case "win":
				aw = "You win";
				break;
			case "win-blackjack":
				aw = "You win<span>Blackjack</span>";
				break;
			case "win-dealer-busted":
				aw = "You win<span>Dealer busted</span>";
				break;
			case "lose":
				aw = "You loose";
				break;
			case "lose-blackjack":
				aw = "You loose<span>Blackjack</span>";
				break;
			case "lose-busted":
				aw = "You loose<span>Busted</span>";
				break;
			case "push":
				aw = "Push<span>No winner</span>";
				break;
			case "game-over":
				aw = "Game over";
				break;
			default:
				aw = "<span>Something broke, don’t know what happened...</span>";
				break;
		}
		ax.innerHTML = aw;
		v.after(ax);
	};
	var u = function ()
	{
		var au = U.sum(),
			av = al.sum();
		if (av > 21)
		{
			af("win-dealer-busted");
		} else
		{
			if (av > au)
			{
				aj("lose");
			} else
			{
				if (au > av)
				{
					af("win");
				} else
				{
					if (au == av)
					{
						ag("push");
					}
				}
			}
		}
	};
	var ak = function ()
	{
		H("game-over");
		G = true;
		var au = document.createElement("div");
		au.id = "overlay";
		$("body").append(au);
	};
	var m = function ()
	{
		T = false;
		o.show();
		g.hide();
		V.removeClass("disabled");
		b.each(function (av)
		{
			var au = $(this);
			if (au.data("value") > ap)
			{
				au.addClass("desactivate");
				var ax = b.removeClass("bet").not(".desactivate");
				if (ax.size() == 0)
				{
					ak();
				} else
				{
					var aw = ax.last();
					aw.addClass("bet");
					ab(aw.data("value"));
					V.prepend(aw);
				}
			} else
			{
				if (au.hasClass("desactivate"))
				{
					au.removeClass("desactivate");
				}
			}
		});
	};
	var x = function ()
	{
		w = false;
		E("front", "player", function ()
		{
			E("front", "dealer", function ()
			{
				E("front", "player", function ()
				{
					E("back", "dealer", function ()
					{
						B();
					});
				});
			});
		});
		o.hide();
		g.show();
		V.addClass("disabled");
	};
	var B = function ()
	{
		var au = U.sum(),
			av = al.sum();
		if (au == 21 && av == 21)
		{
			ag("Push - No winner");
		} else
		{
			if (au == 21)
			{
				af("win-blackjack");
			} else
			{
				if (av == 21)
				{
					aj("lose-blackjack");
					K();
				}
			}
		}
	};
	var L = function (au)
	{
		if (au == 1)
		{
			au = 11;
			i++;
		}
		U.push(au);
		M.html(t());
	};
	var t = function ()
	{
		var au = U.sum();
		if (au > 21 && i > 0)
		{
			U.splice(U.indexOf(11), 1, 1);
			i--;
			au = t();
		}
		return au;
	};
	var K = function ()
	{
		var av = $(".back"),
			ax = av.data("id"),
			aw = W[ax],
			au = J(ax, aw.type, aw.value, "front");
		au.css({
			left: 10 * av.index() + "%",
			"z-index": 50 - av.index()
		});
		av.after(au).remove();
		ai.html(N());
	};
	var ah = function (au)
	{
		if (au == 1)
		{
			au = 11;
			Q++;
		}
		al.push(au);
	};
	var N = function ()
	{
		var au = al.sum();
		if (au > 21 && Q > 0)
		{
			al.splice(al.indexOf(11), 1, 1);
			Q--;
			au = N();
		}
		return au;
	};
	var e = function ()
	{
		b.bind("click", function (av)
		{
			var au = $(this);
			if (T || au.hasClass("desactivate"))
			{
				return;
			}
			b.removeClass("bet");
			au.addClass("bet");
			ab(au.data("value"));
			V.prepend(au);
		});
	};
	var ab = function (au)
	{
		if (T)
		{
			return;
		}
		A = au;
	};
	var s = function (au)
	{
		ap += au * A;
		ar.html((ap / 10) + "k");
	};
	return S;
})();
Array.prototype.shuffle = function ()
{
	for (var b, a, c = this.length; c; b = Math.floor(Math.random() * c), a = this[--c], this[c] = this[b], this[b] = a) { }
};
Array.prototype.sum = function ()
{
	for (var b = 0, a = this.length; a; b += this[--a]) { }
	return b;
};
var BrowserDetect = {
	init: function ()
	{
		this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
		this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator.appVersion) || "an unknown version";
		this.OS = this.searchString(this.dataOS) || "an unknown OS";
		var a = document.documentElement;
		a.setAttribute("browser", this.browser);
		a.setAttribute("version", this.version);
		a.setAttribute("os", this.OS);
	},
	searchString: function (d)
	{
		for (var a = 0; a < d.length; a++)
		{
			var b = d[a].string;
			var c = d[a].prop;
			this.versionSearchString = d[a].versionSearch || d[a].identity;
			if (b)
			{
				if (b.indexOf(d[a].subString) != -1)
				{
					return d[a].identity;
				}
			} else
			{
				if (c)
				{
					return d[a].identity;
				}
			}
		}
	},
	searchVersion: function (b)
	{
		var a = b.indexOf(this.versionSearchString);
		if (a == -1)
		{
			return;
		}
		return parseFloat(b.substring(a + this.versionSearchString.length + 1));
	},
	dataBrowser: [{
		string: navigator.vendor,
		subString: "Apple",
		identity: "Safari",
		versionSearch: "Version"
	}, {
		string: navigator.userAgent,
		subString: "Firefox",
		identity: "Firefox"
	}],
	dataOS: [{
		string: navigator.userAgent,
		subString: "iPhone",
		identity: "iPhone/iPod"
	}]
};
BrowserDetect.init();