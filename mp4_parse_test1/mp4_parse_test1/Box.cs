using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mp4_parse_test1
{
	// http://www.mp4ra.org/atoms.html
	public enum BoxType : uint
	{
		Unknown = uint.MaxValue,
		ainf = ('a' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		avcn = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('n' << 0),
		bloc = ('b' << 24) | ('l' << 16) | ('o' << 8) | ('c' << 0),
		bpcc = ('b' << 24) | ('p' << 16) | ('c' << 8) | ('c' << 0),
		buff = ('b' << 24) | ('u' << 16) | ('f' << 8) | ('f' << 0),
		bxml = ('b' << 24) | ('x' << 16) | ('m' << 8) | ('l' << 0),
		ccid = ('c' << 24) | ('c' << 16) | ('i' << 8) | ('d' << 0),
		cdef = ('c' << 24) | ('d' << 16) | ('e' << 8) | ('f' << 0),
		clip = ('c' << 24) | ('l' << 16) | ('i' << 8) | ('p' << 0),
		cmap = ('c' << 24) | ('m' << 16) | ('a' << 8) | ('p' << 0),
		co64 = ('c' << 24) | ('o' << 16) | ('6' << 8) | ('4' << 0),
		coin = ('c' << 24) | ('o' << 16) | ('i' << 8) | ('n' << 0),
		colr = ('c' << 24) | ('o' << 16) | ('l' << 8) | ('r' << 0),
		crgn = ('c' << 24) | ('r' << 16) | ('g' << 8) | ('n' << 0),
		crhd = ('c' << 24) | ('r' << 16) | ('h' << 8) | ('d' << 0),
		cslg = ('c' << 24) | ('s' << 16) | ('l' << 8) | ('g' << 0),
		ctab = ('c' << 24) | ('t' << 16) | ('a' << 8) | ('b' << 0),
		ctts = ('c' << 24) | ('t' << 16) | ('t' << 8) | ('s' << 0),
		cvru = ('c' << 24) | ('v' << 16) | ('r' << 8) | ('u' << 0),
		dinf = ('d' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		dref = ('d' << 24) | ('r' << 16) | ('e' << 8) | ('f' << 0),
		dsgd = ('d' << 24) | ('s' << 16) | ('g' << 8) | ('d' << 0),
		dstg = ('d' << 24) | ('s' << 16) | ('t' << 8) | ('g' << 0),
		edts = ('e' << 24) | ('d' << 16) | ('t' << 8) | ('s' << 0),
		elst = ('e' << 24) | ('l' << 16) | ('s' << 8) | ('t' << 0),
		emsg = ('e' << 24) | ('m' << 16) | ('s' << 8) | ('g' << 0),
		fdel = ('f' << 24) | ('d' << 16) | ('e' << 8) | ('l' << 0),
		feci = ('f' << 24) | ('e' << 16) | ('c' << 8) | ('i' << 0),
		fecr = ('f' << 24) | ('e' << 16) | ('c' << 8) | ('r' << 0),
		fiin = ('f' << 24) | ('i' << 16) | ('i' << 8) | ('n' << 0),
		fire = ('f' << 24) | ('i' << 16) | ('r' << 8) | ('e' << 0),
		fpar = ('f' << 24) | ('p' << 16) | ('a' << 8) | ('r' << 0),
		free = ('f' << 24) | ('r' << 16) | ('e' << 8) | ('e' << 0),
		frma = ('f' << 24) | ('r' << 16) | ('m' << 8) | ('a' << 0),
		ftyp = ('f' << 24) | ('t' << 16) | ('y' << 8) | ('p' << 0),
		gitn = ('g' << 24) | ('i' << 16) | ('t' << 8) | ('n' << 0),
		grpi = ('g' << 24) | ('r' << 16) | ('p' << 8) | ('i' << 0),
		hdlr = ('h' << 24) | ('d' << 16) | ('l' << 8) | ('r' << 0),
		hmhd = ('h' << 24) | ('m' << 16) | ('h' << 8) | ('d' << 0),
		hpix = ('h' << 24) | ('p' << 16) | ('i' << 8) | ('x' << 0),
		icnu = ('i' << 24) | ('c' << 16) | ('n' << 8) | ('u' << 0),
		ID32 = ('I' << 24) | ('D' << 16) | ('3' << 8) | ('2' << 0),
		idat = ('i' << 24) | ('d' << 16) | ('a' << 8) | ('t' << 0),
		ihdr = ('i' << 24) | ('h' << 16) | ('d' << 8) | ('r' << 0),
		iinf = ('i' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		iloc = ('i' << 24) | ('l' << 16) | ('o' << 8) | ('c' << 0),
		imap = ('i' << 24) | ('m' << 16) | ('a' << 8) | ('p' << 0),
		imif = ('i' << 24) | ('m' << 16) | ('i' << 8) | ('f' << 0),
		infe = ('i' << 24) | ('n' << 16) | ('f' << 8) | ('e' << 0),
		infu = ('i' << 24) | ('n' << 16) | ('f' << 8) | ('u' << 0),
		iods = ('i' << 24) | ('o' << 16) | ('d' << 8) | ('s' << 0),
		iphd = ('i' << 24) | ('p' << 16) | ('h' << 8) | ('d' << 0),
		ipmc = ('i' << 24) | ('p' << 16) | ('m' << 8) | ('c' << 0),
		ipro = ('i' << 24) | ('p' << 16) | ('r' << 8) | ('o' << 0),
		iref = ('i' << 24) | ('r' << 16) | ('e' << 8) | ('f' << 0),
		jP   = ('j' << 24) | ('P' << 16) | (' ' << 8) | (' ' << 0),
		jp2c = ('j' << 24) | ('p' << 16) | ('2' << 8) | ('c' << 0),
		jp2h = ('j' << 24) | ('p' << 16) | ('2' << 8) | ('h' << 0),
		jp2i = ('j' << 24) | ('p' << 16) | ('2' << 8) | ('i' << 0),
		kmat = ('k' << 24) | ('m' << 16) | ('a' << 8) | ('t' << 0),
		leva = ('l' << 24) | ('e' << 16) | ('v' << 8) | ('a' << 0),
		load = ('l' << 24) | ('o' << 16) | ('a' << 8) | ('d' << 0),
		lrcu = ('l' << 24) | ('r' << 16) | ('c' << 8) | ('u' << 0),
		m7hd = ('m' << 24) | ('7' << 16) | ('h' << 8) | ('d' << 0),
		matt = ('m' << 24) | ('a' << 16) | ('t' << 8) | ('t' << 0),
		mdat = ('m' << 24) | ('d' << 16) | ('a' << 8) | ('t' << 0),
		mdhd = ('m' << 24) | ('d' << 16) | ('h' << 8) | ('d' << 0),
		mdia = ('m' << 24) | ('d' << 16) | ('i' << 8) | ('a' << 0),
		mdri = ('m' << 24) | ('d' << 16) | ('r' << 8) | ('i' << 0),
		meco = ('m' << 24) | ('e' << 16) | ('c' << 8) | ('o' << 0),
		mehd = ('m' << 24) | ('e' << 16) | ('h' << 8) | ('d' << 0),
		mere = ('m' << 24) | ('e' << 16) | ('r' << 8) | ('e' << 0),
		meta = ('m' << 24) | ('e' << 16) | ('t' << 8) | ('a' << 0),
		mfhd = ('m' << 24) | ('f' << 16) | ('h' << 8) | ('d' << 0),
		mfra = ('m' << 24) | ('f' << 16) | ('r' << 8) | ('a' << 0),
		mfro = ('m' << 24) | ('f' << 16) | ('r' << 8) | ('o' << 0),
		minf = ('m' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		mjhd = ('m' << 24) | ('j' << 16) | ('h' << 8) | ('d' << 0),
		moof = ('m' << 24) | ('o' << 16) | ('o' << 8) | ('f' << 0),
		moov = ('m' << 24) | ('o' << 16) | ('o' << 8) | ('v' << 0),
		mvcg = ('m' << 24) | ('v' << 16) | ('c' << 8) | ('g' << 0),
		mvci = ('m' << 24) | ('v' << 16) | ('c' << 8) | ('i' << 0),
		mvex = ('m' << 24) | ('v' << 16) | ('e' << 8) | ('x' << 0),
		mvhd = ('m' << 24) | ('v' << 16) | ('h' << 8) | ('d' << 0),
		mvra = ('m' << 24) | ('v' << 16) | ('r' << 8) | ('a' << 0),
		nmhd = ('n' << 24) | ('m' << 16) | ('h' << 8) | ('d' << 0),
		ochd = ('o' << 24) | ('c' << 16) | ('h' << 8) | ('d' << 0),
		odaf = ('o' << 24) | ('d' << 16) | ('a' << 8) | ('f' << 0),
		odda = ('o' << 24) | ('d' << 16) | ('d' << 8) | ('a' << 0),
		odhd = ('o' << 24) | ('d' << 16) | ('h' << 8) | ('d' << 0),
		odhe = ('o' << 24) | ('d' << 16) | ('h' << 8) | ('e' << 0),
		odrb = ('o' << 24) | ('d' << 16) | ('r' << 8) | ('b' << 0),
		odrm = ('o' << 24) | ('d' << 16) | ('r' << 8) | ('m' << 0),
		odtt = ('o' << 24) | ('d' << 16) | ('t' << 8) | ('t' << 0),
		ohdr = ('o' << 24) | ('h' << 16) | ('d' << 8) | ('r' << 0),
		padb = ('p' << 24) | ('a' << 16) | ('d' << 8) | ('b' << 0),
		paen = ('p' << 24) | ('a' << 16) | ('e' << 8) | ('n' << 0),
		pclr = ('p' << 24) | ('c' << 16) | ('l' << 8) | ('r' << 0),
		pdin = ('p' << 24) | ('d' << 16) | ('i' << 8) | ('n' << 0),
		pitm = ('p' << 24) | ('i' << 16) | ('t' << 8) | ('m' << 0),
		pnot = ('p' << 24) | ('n' << 16) | ('o' << 8) | ('t' << 0),
		prft = ('p' << 24) | ('r' << 16) | ('f' << 8) | ('t' << 0),
		pssh = ('p' << 24) | ('s' << 16) | ('s' << 8) | ('h' << 0),
		res  = ('r' << 24) | ('e' << 16) | ('s' << 8) | (' ' << 0),
		resc = ('r' << 24) | ('e' << 16) | ('s' << 8) | ('c' << 0),
		resd = ('r' << 24) | ('e' << 16) | ('s' << 8) | ('d' << 0),
		rinf = ('r' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		saio = ('s' << 24) | ('a' << 16) | ('i' << 8) | ('o' << 0),
		saiz = ('s' << 24) | ('a' << 16) | ('i' << 8) | ('z' << 0),
		sbgp = ('s' << 24) | ('b' << 16) | ('g' << 8) | ('p' << 0),
		schi = ('s' << 24) | ('c' << 16) | ('h' << 8) | ('i' << 0),
		schm = ('s' << 24) | ('c' << 16) | ('h' << 8) | ('m' << 0),
		sdep = ('s' << 24) | ('d' << 16) | ('e' << 8) | ('p' << 0),
		sdhd = ('s' << 24) | ('d' << 16) | ('h' << 8) | ('d' << 0),
		sdtp = ('s' << 24) | ('d' << 16) | ('t' << 8) | ('p' << 0),
		sdvp = ('s' << 24) | ('d' << 16) | ('v' << 8) | ('p' << 0),
		segr = ('s' << 24) | ('e' << 16) | ('g' << 8) | ('r' << 0),
		senc = ('s' << 24) | ('e' << 16) | ('n' << 8) | ('c' << 0),
		sgpd = ('s' << 24) | ('g' << 16) | ('p' << 8) | ('d' << 0),
		sidx = ('s' << 24) | ('i' << 16) | ('d' << 8) | ('x' << 0),
		sinf = ('s' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		skip = ('s' << 24) | ('k' << 16) | ('i' << 8) | ('p' << 0),
		smhd = ('s' << 24) | ('m' << 16) | ('h' << 8) | ('d' << 0),
		srmb = ('s' << 24) | ('r' << 16) | ('m' << 8) | ('b' << 0),
		srmc = ('s' << 24) | ('r' << 16) | ('m' << 8) | ('c' << 0),
		srpp = ('s' << 24) | ('r' << 16) | ('p' << 8) | ('p' << 0),
		ssix = ('s' << 24) | ('s' << 16) | ('i' << 8) | ('x' << 0),
		stbl = ('s' << 24) | ('t' << 16) | ('b' << 8) | ('l' << 0),
		stco = ('s' << 24) | ('t' << 16) | ('c' << 8) | ('o' << 0),
		stdp = ('s' << 24) | ('t' << 16) | ('d' << 8) | ('p' << 0),
		sthd = ('s' << 24) | ('t' << 16) | ('h' << 8) | ('d' << 0),
		strd = ('s' << 24) | ('t' << 16) | ('r' << 8) | ('d' << 0),
		stri = ('s' << 24) | ('t' << 16) | ('r' << 8) | ('i' << 0),
		stsc = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('c' << 0),
		stsd = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('d' << 0),
		stsg = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('g' << 0),
		stsh = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('h' << 0),
		stss = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('s' << 0),
		stsz = ('s' << 24) | ('t' << 16) | ('s' << 8) | ('z' << 0),
		stts = ('s' << 24) | ('t' << 16) | ('t' << 8) | ('s' << 0),
		styp = ('s' << 24) | ('t' << 16) | ('y' << 8) | ('p' << 0),
		stz2 = ('s' << 24) | ('t' << 16) | ('z' << 8) | ('2' << 0),
		subs = ('s' << 24) | ('u' << 16) | ('b' << 8) | ('s' << 0),
		swtc = ('s' << 24) | ('w' << 16) | ('t' << 8) | ('c' << 0),
		tfad = ('t' << 24) | ('f' << 16) | ('a' << 8) | ('d' << 0),
		tfdt = ('t' << 24) | ('f' << 16) | ('d' << 8) | ('t' << 0),
		tfhd = ('t' << 24) | ('f' << 16) | ('h' << 8) | ('d' << 0),
		tfma = ('t' << 24) | ('f' << 16) | ('m' << 8) | ('a' << 0),
		tfra = ('t' << 24) | ('f' << 16) | ('r' << 8) | ('a' << 0),
		tibr = ('t' << 24) | ('i' << 16) | ('b' << 8) | ('r' << 0),
		tiri = ('t' << 24) | ('i' << 16) | ('r' << 8) | ('i' << 0),
		tkhd = ('t' << 24) | ('k' << 16) | ('h' << 8) | ('d' << 0),
		traf = ('t' << 24) | ('r' << 16) | ('a' << 8) | ('f' << 0),
		trak = ('t' << 24) | ('r' << 16) | ('a' << 8) | ('k' << 0),
		tref = ('t' << 24) | ('r' << 16) | ('e' << 8) | ('f' << 0),
		trex = ('t' << 24) | ('r' << 16) | ('e' << 8) | ('x' << 0),
		trgr = ('t' << 24) | ('r' << 16) | ('g' << 8) | ('r' << 0),
		trik = ('t' << 24) | ('r' << 16) | ('i' << 8) | ('k' << 0),
		trun = ('t' << 24) | ('r' << 16) | ('u' << 8) | ('n' << 0),
		udta = ('u' << 24) | ('d' << 16) | ('t' << 8) | ('a' << 0),
		uinf = ('u' << 24) | ('i' << 16) | ('n' << 8) | ('f' << 0),
		UITS = ('U' << 24) | ('I' << 16) | ('T' << 8) | ('S' << 0),
		ulst = ('u' << 24) | ('l' << 16) | ('s' << 8) | ('t' << 0),
		url  = ('u' << 24) | ('r' << 16) | ('l' << 8) | (' ' << 0),
		uuid = ('u' << 24) | ('u' << 16) | ('i' << 8) | ('d' << 0),
		vmhd = ('v' << 24) | ('m' << 16) | ('h' << 8) | ('d' << 0),
		vwdi = ('v' << 24) | ('w' << 16) | ('d' << 8) | ('i' << 0),
		xml  = ('x' << 24) | ('m' << 16) | ('l' << 8) | (' ' << 0),

		// TODO: どう扱うべきか分からないから一旦追加
		avc1 = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('1' << 0),
		mp4a = ('m' << 24) | ('p' << 16) | ('4' << 8) | ('a' << 0),
		esds = ('e' << 24) | ('s' << 16) | ('d' << 8) | ('s' << 0),
	}

	// http://www.mp4ra.org/codecs.html
	public enum SampleEntryCode : uint
	{
		//3gvo = ('3' << 24) | ('g' << 16) | ('v' << 8) | ('o' << 0),
		ac_3 = ('a' << 24) | ('c' << 16) | ('-' << 8) | ('3' << 0),
		ac_4 = ('a' << 24) | ('c' << 16) | ('-' << 8) | ('4' << 0),
		alac = ('a' << 24) | ('l' << 16) | ('a' << 8) | ('c' << 0),
		alaw = ('a' << 24) | ('l' << 16) | ('a' << 8) | ('w' << 0),
		avc1 = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('1' << 0),
		avc2 = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('2' << 0),
		avc3 = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('3' << 0),
		avc4 = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('4' << 0),
		avcp = ('a' << 24) | ('v' << 16) | ('c' << 8) | ('p' << 0),
		dra1 = ('d' << 24) | ('r' << 16) | ('a' << 8) | ('1' << 0),
		drac = ('d' << 24) | ('r' << 16) | ('a' << 8) | ('c' << 0),
		dtsc = ('d' << 24) | ('t' << 16) | ('s' << 8) | ('c' << 0),
		dtse = ('d' << 24) | ('t' << 16) | ('s' << 8) | ('e' << 0),
		dtsh = ('d' << 24) | ('t' << 16) | ('s' << 8) | ('h' << 0),
		dtsl = ('d' << 24) | ('t' << 16) | ('s' << 8) | ('l' << 0),
		dtsp = ('d' << 24) | ('t' << 16) | ('s' << 8) | ('+' << 0), // plus
		dtsm = ('d' << 24) | ('t' << 16) | ('s' << 8) | ('-' << 0), // minus
		dtsx = ('d' << 24) | ('t' << 16) | ('s' << 8) | ('x' << 0),
		dvav = ('d' << 24) | ('v' << 16) | ('a' << 8) | ('v' << 0),
		dvhe = ('d' << 24) | ('v' << 16) | ('h' << 8) | ('e' << 0),
		ec_3 = ('e' << 24) | ('c' << 16) | ('-' << 8) | ('3' << 0),
		enca = ('e' << 24) | ('n' << 16) | ('c' << 8) | ('a' << 0),
		encs = ('e' << 24) | ('n' << 16) | ('c' << 8) | ('s' << 0),
		enct = ('e' << 24) | ('n' << 16) | ('c' << 8) | ('t' << 0),
		encv = ('e' << 24) | ('n' << 16) | ('c' << 8) | ('v' << 0),
		fdp  = ('f' << 24) | ('d' << 16) | ('p' << 8) | (' ' << 0),
		g719 = ('g' << 24) | ('7' << 16) | ('1' << 8) | ('9' << 0),
		g726 = ('g' << 24) | ('7' << 16) | ('2' << 8) | ('6' << 0),
		hvc1 = ('h' << 24) | ('v' << 16) | ('c' << 8) | ('1' << 0),
		hev1 = ('h' << 24) | ('e' << 16) | ('v' << 8) | ('1' << 0),
		ixse = ('i' << 24) | ('x' << 16) | ('s' << 8) | ('e' << 0),
		m2ts = ('m' << 24) | ('2' << 16) | ('t' << 8) | ('s' << 0),
		m4ae = ('m' << 24) | ('4' << 16) | ('a' << 8) | ('e' << 0),
		mett = ('m' << 24) | ('e' << 16) | ('t' << 8) | ('t' << 0),
		metx = ('m' << 24) | ('e' << 16) | ('t' << 8) | ('x' << 0),
		mha1 = ('m' << 24) | ('h' << 16) | ('a' << 8) | ('1' << 0),
		mha2 = ('m' << 24) | ('h' << 16) | ('a' << 8) | ('2' << 0),
		mjp2 = ('m' << 24) | ('j' << 16) | ('p' << 8) | ('2' << 0),
		mlix = ('m' << 24) | ('l' << 16) | ('i' << 8) | ('x' << 0),
		mlpa = ('m' << 24) | ('l' << 16) | ('p' << 8) | ('a' << 0),
		mp4a = ('m' << 24) | ('p' << 16) | ('4' << 8) | ('a' << 0),
		mp4s = ('m' << 24) | ('p' << 16) | ('4' << 8) | ('s' << 0),
		mp4v = ('m' << 24) | ('p' << 16) | ('4' << 8) | ('v' << 0),
		mvc1 = ('m' << 24) | ('v' << 16) | ('c' << 8) | ('1' << 0),
		mvc2 = ('m' << 24) | ('v' << 16) | ('c' << 8) | ('2' << 0),
		mvc3 = ('m' << 24) | ('v' << 16) | ('c' << 8) | ('3' << 0),
		mvc4 = ('m' << 24) | ('v' << 16) | ('c' << 8) | ('4' << 0),
		oksd = ('o' << 24) | ('k' << 16) | ('s' << 8) | ('d' << 0),
		Opus = ('O' << 24) | ('p' << 16) | ('u' << 8) | ('s' << 0),
		pm2t = ('p' << 24) | ('m' << 16) | ('2' << 8) | ('t' << 0),
		prtp = ('p' << 24) | ('r' << 16) | ('t' << 8) | ('p' << 0),
		raw  = ('r' << 24) | ('a' << 16) | ('w' << 8) | (' ' << 0),
		resv = ('r' << 24) | ('e' << 16) | ('s' << 8) | ('v' << 0),
		rm2t = ('r' << 24) | ('m' << 16) | ('2' << 8) | ('t' << 0),
		rrtp = ('r' << 24) | ('r' << 16) | ('t' << 8) | ('p' << 0),
		rsrp = ('r' << 24) | ('s' << 16) | ('r' << 8) | ('p' << 0),
		rtp  = ('r' << 24) | ('t' << 16) | ('p' << 8) | (' ' << 0),
		s263 = ('s' << 24) | ('2' << 16) | ('6' << 8) | ('3' << 0),
		samr = ('s' << 24) | ('a' << 16) | ('m' << 8) | ('r' << 0),
		sawb = ('s' << 24) | ('a' << 16) | ('w' << 8) | ('b' << 0),
		sawp = ('s' << 24) | ('a' << 16) | ('w' << 8) | ('p' << 0),
		sevc = ('s' << 24) | ('e' << 16) | ('v' << 8) | ('c' << 0),
		sm2t = ('s' << 24) | ('m' << 16) | ('2' << 8) | ('t' << 0),
		sqcp = ('s' << 24) | ('q' << 16) | ('c' << 8) | ('p' << 0),
		srtp = ('s' << 24) | ('r' << 16) | ('t' << 8) | ('p' << 0),
		ssmv = ('s' << 24) | ('s' << 16) | ('m' << 8) | ('v' << 0),
		stpp = ('s' << 24) | ('t' << 16) | ('p' << 8) | ('p' << 0),
		svc1 = ('s' << 24) | ('v' << 16) | ('c' << 8) | ('1' << 0),
		svc2 = ('s' << 24) | ('v' << 16) | ('c' << 8) | ('2' << 0),
		svcM = ('s' << 24) | ('v' << 16) | ('c' << 8) | ('M' << 0),
		tc64 = ('t' << 24) | ('c' << 16) | ('6' << 8) | ('4' << 0),
		tmcd = ('t' << 24) | ('m' << 16) | ('c' << 8) | ('d' << 0),
		twos = ('t' << 24) | ('w' << 16) | ('o' << 8) | ('s' << 0),
		tx3g = ('t' << 24) | ('x' << 16) | ('3' << 8) | ('g' << 0),
		ulaw = ('u' << 24) | ('l' << 16) | ('a' << 8) | ('w' << 0),
		urim = ('u' << 24) | ('r' << 16) | ('i' << 8) | ('m' << 0),
		vc_1 = ('v' << 24) | ('c' << 16) | ('-' << 8) | ('1' << 0),
		wvtt = ('w' << 24) | ('v' << 16) | ('t' << 8) | ('t' << 0),
	}

	public static class HandlerTypes
	{
		public const string Sound = "soun";
		public const string Video = "vide";
		public const string Hint = "hint";
		public const string Metadata = "meta";
		public const string AuxiliaryVideo = "auxv";
	}

	class BoxNode
	{
		public long Offset { get; set; }
		public uint Size { get; set; }
		public BoxType Type { get; set; }
		public BoxNode Parent { get; set; }
		public List<BoxNode> Children { get; private set; }
		public int Level { get; set; }
		public bool IsRoot { get { return Level == 0; } }
		public BoxNode()
		{
			Children = new List<BoxNode>();
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(StringUtils.FromBinary((uint)Type));
			sb.AppendFormat(" {{ offset:{0:#,#}, size:{1:#,#} }}", Offset, Size);
			return sb.ToString();
		}

		public T As<T>()
			where T : BoxNode, new()
		{
			var newNode = new T();
			newNode.Offset = Offset;
			newNode.Size = Size;
			newNode.Type = Type;
			newNode.Parent = Parent;
			newNode.Children.AddRange(Children);
			newNode.Level = Level;
			return newNode;
		}
	}

	class FullBoxNode : BoxNode
	{
		public byte Version { get; set; }
		public UInt32 Flags { get; set; }

		public FullBoxNode()
		{
		}
	}

	class MoovBoxNode : BoxNode
	{
		public MoovBoxNode()
		{
		}
	}

	class MvhdBoxNode : FullBoxNode
	{
		public DateTime CreationTime { get; set; }
		public DateTime ModificationTime { get; set; }
		public UInt32 TimeScale { get; set; }
		public UInt64 Duration { get; set; }
		public Double Rate { get; set; }
		public Single Volume { get; set; }
		public UInt32 NextTrackId { get; set; }

		public MvhdBoxNode()
		{
		}
	}

	class HdlrBoxNode : FullBoxNode
	{
		public string HandlerType { get; set; }
		public string Name { get; set; }

		public HdlrBoxNode()
		{
		}
	}

	class StsdBoxNode : BoxNode
	{
		public uint SampleEntries { get; set; }

		public StsdBoxNode()
		{
		}
	}

	class SampleEntryNode : BoxNode
	{
		public byte[] Reserved { get; private set; }
		public UInt16 DataReferenceIndex { get; set; }

		public SampleEntryNode()
		{
			Reserved = new byte[6];
		}
	}

	class VisualSampleEntryNode : SampleEntryNode
	{
		public UInt16 PreDefined { get; set; }
		public UInt16 Reserved2 { get; set; }
		public UInt32[] PreDefined2 { get; set; }
		public UInt16 Width { get; set; }
		public UInt16 Height { get; set; }
		public UInt32 HorizontalResolution { get; set; }
		public UInt32 VerticalResolution { get; set; }
		public UInt32 Reserved3 { get; set; }
		public UInt16 FrameCount { get; set; }
		public string CompressorName { get; set; }
		public UInt16 Depth { get; set; }
		public Int32 PreDefined3 { get; set; }

		public VisualSampleEntryNode()
		{
			PreDefined2 = new UInt32[3];
			HorizontalResolution = 0x00480000 >> 16;
			VerticalResolution = 0x00480000 >> 16;
			FrameCount = 1;
			Depth = 0x0018 >> 8;
			PreDefined3 = -1;
		}
	}

	class AudioSampleEntryNode : SampleEntryNode
	{
		public new UInt32[] Reserved { get; set; }
		public UInt16 ChannelCount { get; set; }
		public UInt16 SampleSize { get; set; }
		public UInt16 PreDefined { get; set; }
		public UInt16 Reserved2 { get; set; }
		public UInt32 SampleRate { get; set; }

		public AudioSampleEntryNode()
		{
			Reserved = new UInt32[2];
			ChannelCount = 2;
			SampleSize = 16;
		}
	}

	// http://xhelmboyx.tripod.com/formats/mp4-layout.txt
	// TODO: esds box. 複数のタグが管理されるみたい・・・どう扱うのか・・・
	class ESDescriptorBoxNode : FullBoxNode
	{
		public byte Tag { get; set; }
		public byte TagSize { get; set; }
		public UInt16 ESID { get; set; }
		public byte StreamPriority { get; set; }
	}
}
