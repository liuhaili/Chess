using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer.Fight.AI
{
    public class AIUserRandomAttr
    {
        private static string firstName = @"赵,钱,孙,李,周,吴,郑,王,冯,陈,褚,卫,蒋,
            沈,韩,杨,朱,秦,尤,许,何,吕,施,张,孔,曹,严,华,金,魏,陶,姜, 戚,谢,邹,喻,
            柏,水,窦,章,云,苏,潘,葛,奚,范,彭,郎,鲁,韦,昌,马,苗,凤,花,方,俞,任,袁,柳,
            丰,鲍,史,唐, 费,廉,岑,薛,雷,贺,倪,汤,滕,殷,罗,毕,郝,邬,安,常,乐,于,时,
            傅,皮,卞,齐,康,伍,余,元,卜,顾,孟,平,黄, 和,穆,萧,尹,姚,邵,湛,汪,祁,毛,
            禹,狄,米,贝,明,臧,计,伏,成,戴,谈,宋,茅,庞,熊,纪,舒,屈,项,祝,董,梁, 杜,
            阮,蓝,闵,席,季,麻,强,贾,路,娄,危,江,童,颜,郭,梅,盛,林,刁,钟,徐,丘,骆,高,
            夏,蔡,田,樊,胡,凌,霍, 虞,万,支,柯,昝,管,卢,莫,经,房,裘,缪,干,解,应,宗,丁,
            宣,贲,邓,郁,单,杭,洪,包,诸,左,石,崔,吉,钮,龚, 程,嵇,邢,滑,裴,陆,荣,翁,荀,
            羊,於,惠,甄,麴,家,封,芮,羿,储,靳,汲,邴,糜,松,井,段,富,巫,乌,焦,巴,弓, 牧,
            隗,山,谷,车,侯,宓,蓬,全,郗,班,仰,秋,仲,伊,宫,宁,仇,栾,暴,甘,钭,厉,戌,祖,
            武,符,刘,景,詹,束,龙, 叶,幸,司,韶,郜,黎,蓟,薄,印,宿,白,怀,蒲,邰,从,鄂,索,
            咸,籍,赖,卓,蔺,屠,蒙,池,乔,阴,郁,胥,能,苍,双, 闻,莘,党,翟,谭,贡,劳,逢,姬,
            申,扶,堵,冉,宰,郦,雍,郤,璩,桑,桂,濮,牛,寿,通,边,扈,燕,冀,郏,浦,尚,农, 温,
            别,庄,晏,柴,瞿,阎,充,慕,连,茹,习,宦,艾,鱼,容,向,古,易,慎,戈,廖,庾,终,暨,
            居,衡,步,都,耿,满,弘, 匡,国,文,寇,广,禄,阙,东,欧,殳,沃,利,蔚,越,菱,隆,师,
            巩,厍,聂,晃,勾,敖,融,冷,訾,辛,阚,那,简,饶,空, 曾,毋,沙,乜,养,鞠,须,丰,巢,
            关,蒯,相,查,后,荆,红,游,竺,权,逯,盖,益,桓,公, 万俟,司马,上官,欧阳,夏侯,
            诸葛,闻人,东方,赫连,皇甫,尉迟,公羊,澹台,公冶,宗政,濮阳,淳于,单于,太叔,
            申屠,公孙,仲孙,轩辕,令狐,钟离,宇文,长孙,慕容,司徒,司空";

        private static string lastName = @"努,迪,立,林,维,吐,丽,新,涛,米,亚,克,湘,明,
            白,玉,代,孜,霖,霞,加,永,卿,约,小,刚,光,峰,春,基,木,国,娜,晓,兰,阿,伟,英,元,
            音,拉,亮,玲,木,兴,成,尔,远,东,华,旭,迪,吉,高,翠,莉,云,华,军,荣,柱,科,生,昊,
            耀,汤,胜,坚,仁,学,荣,延,成,庆,音,初,杰,宪,雄,久,培,祥,胜,梅,顺,涛,西,库,康,
            温,校,信,志,图,艾,赛,潘,多,振,伟,继,福,柯,雷,田,也,勇,乾,其,买,姚,杜,关,陈,
            静,宁,春,马,德,水,梦,晶,精,瑶,朗,语,日,月,星,河,飘,渺,星,空,如,萍,棕,影,南,北";
        private static string nationName = @"汉族,壮族,满族,回族,苗族,维吾尔族,土家族,
            彝族,蒙古族,藏族,布依族,侗族,瑶族,朝鲜族,白族,哈尼族,哈萨克族,黎族,傣族,畲族,
            傈僳族,仡佬族,东乡族,高山族,拉祜族,水族,佤族,纳西族,羌族,土族,仫佬族,锡伯族,
            柯尔克孜族,达斡尔族,景颇族,毛南族,撒拉族,布朗族,塔吉克族,阿昌族,普米族,鄂温克族
            ,怒族,京族,基诺族,德昂族,保安族,俄罗斯族,裕固族,乌兹别克族,门巴族,鄂伦春族,
            独龙族,塔塔尔族,赫哲族,珞巴族";
        static Random rnd = new Random((int)DateTime.Now.ToFileTimeUtc());
        static List<string> faceList = new List<string>();
        static AIUserRandomAttr()
        {
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/E3A72AB2791A82CB1B00AA0F2822C48E/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/ACCE4A1D28952C76F9BC835A0BC8A89C/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/2C1A1B23427EF722FA7A4972C7546C73/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C21222FA23697770C03E89ED1EA7B9FD/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/50B5054F7CC1D9E3F01CD5B1B6CECA83/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/7E03AD6BE26BDE1FFA47BD8982680C44/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B2923B885989BADF544EFD037EDBA523/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/D4BA0889946F3D46EE40FEC4B70C0351/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/CAE5A6B9AA46C2FBF38BC756225BE77E/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/81931B0B6405823A72AF6C8E62A291EC/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/1376F2F513BDBF23BDB8666007A5C276/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/0F8F48E29863FD104B639BB4AC87EFEF/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/2BD791CA2232AA4715ECA7A30CDC18FB/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/3F6EB57F15E5ADA40E4FAD6D8FF865B3/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/9FD99A83FC5FC7E77943D72C4971804D/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C2455005E52D110EF483340EB2EF7147/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/96906DDDC9785065448F4CB898A5C3F0/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/33756459EB2AB2B8CA6F957BC3894C6E/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/11904D322633E78E8F3410F759E2A2A2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/7AE75C272B6916A319BE7BEBD6E18423/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/30213E9CD469919F00FB3AE9ECDE8885/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/CF69F32560BCB87928FADF0AE6E64879/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/878A27308ECD54321F2D9E594A7A45AC/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C587C192AED1F4C5C6D9DE0C82629693/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/856AA668A294A262B73BAD9C3AAD1E5F/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/0ADDA6AFD7DE7585962DE8ED767FE9D2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/CA7C9CCDA251F5B3CA42F52074CDEF49/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/BA87309042B82FD18B6F41FA95C8C118/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/8BF789795FF3987F0F88BB0BE55697C2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/4B0ABB39F0E1CDD39CC58A87080F32D6/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C31091F0C17471366D1038189F6EEE67/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/AAD276ED09A281C02EFB6165E0EB0749/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/24AB4BBC809DD49669A4813F3A9AD1A1/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/CC58310713B430F4AC946E3778EA8906/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/4878BB22E0C8DD757F6B9BEC7AA3EEB7/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/08FD22E5D35C5BB65DB52FCDD973BEFE/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/A90F80582E4AFCE33DC3B89B50E880CB/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/301657BDE37BF55D838F13543C5C0FBC/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/8A790A91A665DB0A9F7684BEA6C09F1A/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/3C33630A2D112F643E686B7033C0DB40/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/29A18CF59BCDAD86CE9E5A4F87B7CAB7/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/04029CC05471786A195F7BD89FD31AF1/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/F59CFB7410B9D2926552E7244A189B7A/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/269FE391E690072CB9ACD2F8C31563A9/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/FEAD67611D4100757A7290B351ADF21A/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/BC025A489FB54D4E937902AEB8E7543A/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/BF8AA9170A2A37DEEDE2C5E8BAA8AD70/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/D0ADC071C7E83094907028505D7FE1C2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/66BA0600A960BFEDB18916C1CCE9D747/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/4AED355E51485E1B00C2A9139E7967E0/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/398488C50FDF48720E62BDC03FA998A3/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/D03016B5B0B71B83C29D5F4567B37E85/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/7C211646AA4B3E234DBBD57718BDCD83/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/12F555F635771BD97A1D0AE050D14185/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/2770E02EE14030D40E25ED9E9AE95F92/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/361AE7466926F5EE9C853AE5E2FDA8C2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/8C8AD8616A39041AFEC34D66CECE9A6D/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B8851A560EF14CA61262F88C13732239/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B7F0343155BC3192B3F68AD7F850FC3F/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/FC08FAF4EF4108F28A241F12E7C3043A/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/4AA8D9F614DD1432D9F9B7AEA4560DE8/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C62D88B94EBA0BE3D0AAC8E3E3346440/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/0BE61468D38FF1DB77887584A3F69552/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/45957AA80C50DC47628E990003FCA29C/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/52A6930790A7FECC423E210E5296BCBE/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/9616A28BBC9DFEB1E91D696E5CF70734/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/FF8DDD74A84637676CAAAAAF4A2BBFFB/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B4E24C5A4E5877465CF5D6F48936DA91/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/494BDD1670F5766DB21B626CBDE8D298/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/5521A96B414077B51C1B6236273B465E/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/2E23D4AD3822A37875251E8B5D0EF2EE/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/884F55087D09FD85968FF7DBC508F2D0/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/3542A4A2DFFE783653644E8E5218CBE9/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/DB121BF2CC32CBAF4E7B1AFEE86901DB/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/DF148E3113057E1A0CE54E16A3487ED6/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/7887496752B469E06B30B4C147E5D703/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/6C5AC3862813D98BD7989E2B7632B3F1/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/969EB0DAB9341E48B99798AF35634262/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/8AADC349F78F2A32CF6CCF1A4FC0563B/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C036A51CB8DC829045FA7D7C05AE2216/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C97910C1025E84CCBB1FB69AEC0F5EB7/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B5FB6A55989FAFD3CEC5DD34EA0911BA/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/2069B9F1C506295F00188385757B7B90/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/F132F43D46896CDD4AE1BC68B7D495B6/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B6CDD3E2BAC33A9585917F4C24C054E8/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/9A0CAA271B0D9346190197A079E625F6/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/D045C3BEC53AAC50A02DA3F1E0825E15/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/9BAF8AF169F2A48DCAA7D4D1F05D63B2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/BA39E5AB83EE21F9F5FB756ABF7F96F0/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/A45EE1F60EEAA6543FA50A650CC33D35/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/87408E36F95BC50F522AA7E27DFAA619/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/7DA9EE7164939AD0B3C0BB1B611DE30B/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/AB515B41D8344EABC47EB880B26B1C60/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/DE5DF9BAFAC872B84F2CFE88CF6250E9/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C1D0B21A9BB2D92F4C9EF983B41D82B7/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B2A296C0903A2AF7575901AB925F6709/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/0A121A2490863EDC9CAFDEB3A74335E5/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/FA9B35FD6329C034E2C728441A1A5251/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/E1587A42DE8D5FD85435FA68CA28A37C/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/9FD1681ECE1EE177DF5C341100514C1D/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/6C1ADC2B16973E7E0E87C191A73487F2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/C03ADB9A9334E2096237BCB5C4DA5A29/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B459C4536E457832EDCB90679C4B9AF6/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/984CA61E8E32FA25FC8B3D7D692B19FF/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/50606C6953E3BC257DC9119843728044/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/947F477B9A80B8A19546E970BE1B80C9/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/B93EE8781F093DD5B4479537D0A49D91/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/665CE4023F3D1D26660471A1BDD7033B/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/E2E2820FDAF9D3DB16C7D94837A31257/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/316F4243DA817EF5718E5E30C675522A/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/009CC29AA91EF27099E49C519D2A5B40/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/3508F2B88045F08BE057F4EFD1324C5F/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/9A05215034A93A5A4CD0F8B5DA21E3D4/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/DE2B701AE33AFA8353795F8CFD25780B/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/2C0F9A4757C18F479F68816B80C21375/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/CE024DF41971304CFA8FF1832F17FE90/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/7127C78A6C90B9D1D73DFE0FCCCC7555/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/E447D8526AAB9CE4F466D3E301EBD315/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/E447D8526AAB9CE4F466D3E301EBD315/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/A6955D1D49E6E7B62838ED490A3712D2/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/680B5C5E4703749DEBE6297416EBA98C/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/52F55058B341CEC1CFDAD8F1CCF87AC9/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/335CFC5F3E1F7CEDD14FB5A4ED005B09/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/EE1EE0840D95C64635897B1C15AA96C4/100");
            faceList.Add("http://q.qlogo.cn/qqapp/1105455181/EF74C36FEA4B5F7EB5B51C1CDB6E78BE/100");
        }


        public static string getRandomName()
        {
            int namelength = 0;
            namelength = rnd.Next(2, 4);
            firstName = firstName.Replace("\n", "");
            firstName = firstName.Replace("\r", "");
            firstName = firstName.Replace(" ", "");
            lastName = lastName.Replace("\r", "");
            lastName = lastName.Replace("\n", "");
            lastName = lastName.Replace(" ", "");
            string name = "";
            string[] FirstName = firstName.Split(',');
            string[] LastName = lastName.Split(',');
            if (namelength == 2)
            {
                name = FirstName[rnd.Next(0, FirstName.Length)] + LastName[rnd.Next(0, LastName.Length)];
            }
            else if (namelength == 3)
            {
                name = FirstName[rnd.Next(0, FirstName.Length)] + LastName[rnd.Next(0, LastName.Length)] + LastName[rnd.Next(0, LastName.Length)];
            }

            return name;
        }
        public static string GetRandomNumber(int startnumber, int endnumber)
        {
            string strNumber = rnd.Next(startnumber, endnumber).ToString();
            return strNumber;
        }
        public static string GetRandomNation()
        {
            nationName = nationName.Replace("\n", "");
            nationName = nationName.Replace("\r", "");
            nationName = nationName.Replace(" ", "");
            string[] nationname = nationName.Split(',');
            string nation = nationname[rnd.Next(0, 55)];
            return nation;
        }
        public static string GetRandomFace()
        {
            int ind= rnd.Next(0, faceList.Count);
            return faceList[ind];
        }
    }
}
