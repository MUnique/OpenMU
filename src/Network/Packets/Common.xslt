<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:pd="http://www.munique.net/OpenMU/PacketDefinitions"
>
  <xsl:variable name='newline'>
    <xsl:text xml:space='preserve'>
</xsl:text>
  </xsl:variable>
  <xsl:variable name="upperCaseLetters">ABCDEFGHIJKLMNOPQRSTUVWXYZ</xsl:variable>
  <xsl:variable name="lowerCaseLetters">abcdefghijklmnopqrstuvwxyz</xsl:variable>
  <xsl:variable name="digits">0123456789</xsl:variable>

  <xsl:template match="pd:Name">
    <xsl:value-of select="translate(., ' ()/-:', '')"/>
  </xsl:template>

  <xsl:template name="LowerCaseName">
    <xsl:value-of select="concat(translate(substring(pd:Name, 1, 1), $upperCaseLetters, $lowerCaseLetters), substring(pd:Name, 2))"/>
  </xsl:template>

  <!-- Type mapping: -->
  <xsl:template match="pd:Type[. = 'Boolean']" mode="type">bool</xsl:template>
  <xsl:template match="pd:Type[. = 'Byte']" mode="type">byte</xsl:template>
  <xsl:template match="pd:Type[. = 'ShortLittleEndian' or . = 'ShortBigEndian']" mode="type">ushort</xsl:template>
  <xsl:template match="pd:Type[. = 'IntegerLittleEndian' or . = 'IntegerBigEndian']" mode="type">uint</xsl:template>
  <xsl:template match="pd:Type[. = 'LongLittleEndian' or . = 'LongBigEndian']" mode="type">ulong</xsl:template>
  <xsl:template match="pd:Type[. = 'Float']" mode="type">float</xsl:template>
  <xsl:template match="pd:Type[. = 'String']" mode="type">string</xsl:template>
  <xsl:template match="pd:Type[. = 'Binary']" mode="type">Span&lt;byte&gt;</xsl:template>
  <xsl:template match="pd:Type[. = 'Enum']" mode="type">
    <xsl:variable name="typeName" select="./../pd:TypeName" />
    <xsl:if test="./../../../pd:Enums/pd:Enum[pd:Name = $typeName]">
      <xsl:value-of select="./../../../pd:Name"/>
      <xsl:text>.</xsl:text>
    </xsl:if>
    <xsl:value-of select="$typeName"/>
  </xsl:template>
  <xsl:template match="pd:Type[. = 'Structure[]']" mode="type">
    <xsl:text>Span&lt;</xsl:text>
    <xsl:value-of select="./../pd:TypeName"/>
    <xsl:text>&gt;</xsl:text>
  </xsl:template>

  <xsl:template match="text()" mode="type"></xsl:template>

  <xsl:template name="string-replace-all">
    <xsl:param name="text" />
    <xsl:param name="replace" />
    <xsl:param name="by" />
    <xsl:choose>
      <xsl:when test="contains($text, $replace)">
        <xsl:value-of select="substring-before($text,$replace)" />
        <xsl:value-of select="$by" />
        <xsl:call-template name="string-replace-all">
          <xsl:with-param name="text"
                          select="substring-after($text,$replace)" />
          <xsl:with-param name="replace" select="$replace" />
          <xsl:with-param name="by" select="$by" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Splits a name by inserting a space before each upper case letter and additionally lowering this letter -->
  <xsl:template name="splitName">
    <xsl:param name="name" />

    <xsl:if test="$name != ''">
      <xsl:variable name="currentLetter" select="substring($name, 1, 1)" />
      <xsl:variable name="isUpperCase" select="contains($upperCaseLetters, $currentLetter)" />
      <xsl:variable name="isDigit" select="contains($digits, $currentLetter)" />
      <xsl:choose>
        <xsl:when test="$isUpperCase">
          <xsl:text xml:space="preserve"> </xsl:text>
          <xsl:value-of select="translate($currentLetter, $upperCaseLetters, $lowerCaseLetters)" />
        </xsl:when>
        <xsl:when test="contains($digits, $currentLetter)">
          <xsl:text xml:space="preserve"> </xsl:text>
          <xsl:value-of select="$currentLetter" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$currentLetter"/>
        </xsl:otherwise>
      </xsl:choose>

      <!-- Call this template again to handle the next letter. -->
      <xsl:call-template name="splitName">
        <xsl:with-param name="name" select="substring-after($name, $currentLetter)" />
      </xsl:call-template>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
