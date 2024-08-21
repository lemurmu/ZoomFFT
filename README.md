# ZoomFFT
 频谱细化ZoomFFT/CZT/抛物线插值
<!-- PROJECT SHIELDS -->
<!-- PROJECT LOGO -->
<br />

<div id="top"></div>
<!--
*** 感谢查看我们的最佳 README 模板，如果你有好的建议，请复刻（fork）本仓库并且创建一个
*** 拉取请求（pull request），或者直接创建一个带「enhancement」标签的议题（issue）。
*** 不要忘记给该项目点一个 star！
*** 再次感谢！现在快去创建一些了不起的东西吧！:D
-->



<!-- 项目 SHIELDS -->
<!--
*** 我们使用了 markdown 「参考风格」的链接以便于阅读。
*** 参考链接是用方括号 [ ] 包围起来的，而非 圆括号 ( )。
*** 请到文档末尾查看 contributors-url、forks-url 等变量的声明。这是一种可选的简洁语法，你可能会想要使用。
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- 项目 LOGO -->
<br />
<div align="center">
  <h3 align="center">ZoomFFT</h3>

  <p align="center">
    频谱细化ZoomFFT/CZT/抛物线插值，估计精确频率！
    <br />
    <a href="https://github.com/lemurmu/ZoomFFT/tree/master/README.md"><strong>浏览文档 »</strong></a>
    <br />
    <br />
    <a href="https://github.com/lemurmu/ZoomFFT/tree/master/ZoomFFT">查看 Demo</a>
    ·
    <a href="https://github.com/lemurmu/ZoomFFT/tree/master/ZoomFFT/issues">反馈 Bug</a>
    ·
    <a href="https://github.com/lemurmu/ZoomFFT/tree/master/ZoomFFT/issues">请求新功能</a>
  </p>
</div>



<!-- 目录 -->
<details>
  <summary>目录</summary>
  <ol>
    <li>
      <a href="#关于本项目">关于本项目</a>
      <ul>
        <li><a href="#构建工具">构建工具</a></li>
      </ul>
    </li>
    <li>
      <a href="#开始">开始</a>
      <ul>
        <li><a href="#依赖">依赖</a></li>
        <li><a href="#安装">安装</a></li>
      </ul>
    </li>
    <li><a href="#使用方法">使用方法</a></li>
    <li><a href="#路线图">路线图</a></li>
    <li><a href="#贡献">贡献</a></li>
    <li><a href="#许可证">许可证</a></li>
    <li><a href="#联系我们">联系我们</a></li>
    <li><a href="#致谢">致谢</a></li>
  </ol>
</details>



<!-- 关于本项目 -->
## 关于本项目

[![产品截图][product-screenshot]](https://github.com/lemurmu/ZoomFFT/tree/blob/Screenshoot/main.png)

在进行DFT的过程中，最后需要对信号的频谱进行采样。经过这种采样所显示出来的频谱仅在各采样点上，而不在此类点上的频谱都显示不出来，
即使在其他点上有重要的峰值也会被忽略，这就是栅栏效应。

[![栅栏效应][product-screenshot]](https://github.com/lemurmu/ZoomFFT/tree/blob/Screenshoot/栅栏效应.png)

这一效应对于周期信号尤为重要，因其频谱是离散的，如处理不当这些离散谱线可能不被显示。

不管是时域采样还是频域采样，都有相应的栅栏效应。只是当时域采样满足采样定理时，栅栏效应不会有什么影响。而频域采样的栅栏效应则影响很大，“挡住”或丢失的频率成分有可能是重要的或具有特征的成分，使信号处理失去意义。

减小栅栏效应可用提高采样间隔也就是频率分辨力的方法来解决。间隔小，频率分辨力高，被“挡住”或丢失的频率成分就会越少。但会增加采样点数，使计算工作量增加。

解决此项矛盾可以采用如下方法：在满足采样定理的前提下，采用频率细化技术(ZOOM），亦可用把时域序列变换成频谱序列的方法。

在对周期信号DFT处理时，解决栅栏效应应以致解决泄露效应的一个极为有效的措施是所谓“整周期截取”。

而对于非周期信号，如果希望减小栅栏效应的影响，尽可能多地观察到谱线，则需要提高频谱的分辨率。

细化技术
在解决栅栏效应中，碰到了一个概念，叫做细化，对与本设计中出现的问题，因为频率分辨率最后已经达到了0.36，频率点也都比较准确，所以出现栅栏效应的可能性较小。

但是，对于栅栏效应和细化技术，还是很值的去探索一下

也对 Zoom-FFT算法 进行一个学习

什么是细化技术?
**细化技术是一种一定频率范围内提高频率分辨率的测量技术，也叫细化傅里叶分析。**即在所选择的频带内，进行与基带分析有同样多的谱线的分析。从而能大大提高频率分辨率。

选带分析时频率分辨率为：

选带傅氏分析（细化）的主要步骤是：

(a) 输入信号先经模拟抗混滤波，滤去所需分析的最高频率即基带分析中的最高分析频率以上的频率成分；

(b) 经过模数转换，变为数字信号序列；

© 将采样信号经数字移频，移频后的Fk处的谱线将落在频率轴上的零频处；

(d) 将移频后的数字信号再经数字低通滤波，滤去所需频带以外的信号；

(e) 对滤波后的信号的时间序列进行重采样，此时分析的是一段小频段为原来的1/M。这样在一小频段上采样，采样量还是N，但采样时间加了M倍，提高了分辩率。

细化FFT技术的应用：
一些不能增加总的采样点数而分辨率又要求精细的场合，细化FFT分析是很有用的。例如：

（a）区分频谱图中间距很近的共振尖峰，用常规分析不能很好分开时，用细化分析就能得到满意的结果。

（b）用于增加信噪比，提高谱值精度，这是由于细化时采用了数字滤波器，混叠与泄漏产生的误差都非常小;

（c ) 用于分离被白噪声淹没的单频信号，由于白噪声的功率谱与频率分辨率有关，每细化一个2倍，白噪声的功率谱值降低3dB，若细化256倍，白噪声功率谱值即下降24 dB，而单频信号的谱线就会被突出出来。
Zoom-FFT根本没有实现“细化“？
首先说频率分辨率的概念，默认指的信号客观的物理可达到的分辨率，即物理分辨率；并非“计算分辨率”

假设信号的采样时间Delta_T定了，信号在频域上的理论最高分辨率也就已经确定了，等于Delta_f=1/Delta_T;

CZT变换
采用FFT算法可以很快算出全部N点DFT值，即Z变换X ( z ) X\left( z \right)X(z)在Z平面单位圆上的全部等间隔取样值。

实际中，也许不需要计算整个单位圆上Z变换的取样，如对于窄带信号，只需要对信号所在的一段频带进行分析，

这时希望频谱的采样集中在这一频带内，以获得较高的分辨率，而频带以外的部分可不考虑，或者对其他围线上的Z变换取样感兴趣，

例如语音信号处理中，需要知道Z变换的极点所在频率，如极点位置离单位圆较远，则其单位圆上的频谱就很平滑，这时很难从中识别出极点所在的频率，

如果采样不是沿单位圆而是沿一条接近这些极点的弧线进行，则在极点所在频率上的频谱将出现明显的尖峰，由此可较准确地测定极点频率。

螺旋线采样是一种适合于这种需要的变换，且可以采用FFT来快速计算，这种变换也称作Chirp-z变换。

<p align="right">(<a href="#top">返回顶部</a>)</p>

### 构建工具

- [scichart](https://www.scichart.com/)
- [handycontrol](https://handyorg.gitee.io/handycontrol/)
- [mathdotnet](https://www.mathdotnet.com/)

<p align="right">(<a href="#top">返回顶部</a>)</p>



<!-- 开始 -->
## 开始

拉取代码编译便可食用。

### 依赖

* .NET Framwork
<!-- 贡献 -->
## 贡献
windfall

<p align="right">(<a href="#top">返回顶部</a>)</p>


<!-- 许可证 -->
## 许可证

根据 MIT 许可证分发。打开 [LICENSE.txt](LICENSE.txt) 查看更多内容。


<p align="right">(<a href="#top">返回顶部</a>)</p>



<!-- 联系我们 -->
## 联系我们



<p align="right">(<a href="#top">返回顶部</a>)</p>



<!-- 致谢 -->
## 致谢

在这里列出你觉得有用的资源，并以此致谢。我已经添加了一些我喜欢的资源，以便你可以快速开始！

* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Malven's Flexbox Cheatsheet](https://flexbox.malven.co/)
* [Malven's Grid Cheatsheet](https://grid.malven.co/)
* [Img Shields](https://shields.io)
* [GitHub Pages](https://pages.github.com)
* [Font Awesome](https://fontawesome.com)
* [React Icons](https://react-icons.github.io/react-icons/search)

<p align="right">(<a href="#top">返回顶部</a>)</p>



<!-- MARKDOWN 链接 & 图片 -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/BreakingAwful/Best-README-Template-zh.svg?style=for-the-badge
[contributors-url]: https://github.com/BreakingAwful/Best-README-Template-zh/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/BreakingAwful/Best-README-Template-zh.svg?style=for-the-badge
[forks-url]: https://github.com/BreakingAwful/Best-README-Template-zh/network/members
[stars-shield]: https://img.shields.io/github/stars/BreakingAwful/Best-README-Template-zh.svg?style=for-the-badge
[stars-url]: https://github.com/BreakingAwful/Best-README-Template-zh/stargazers
[issues-shield]: https://img.shields.io/github/issues/BreakingAwful/Best-README-Template-zh.svg?style=for-the-badge
[issues-url]: https://github.com/BreakingAwful/Best-README-Template-zh/issues
[license-shield]: https://img.shields.io/github/license/BreakingAwful/Best-README-Template-zh.svg?style=for-the-badge
[license-url]: https://github.com/BreakingAwful/Best-README-Template-zh/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/othneildrew
[product-screenshot]: images/screenshot.png
