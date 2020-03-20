<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:d="http://www.munique.net/OpenMU/PacketDefinitions"
                version="3.0">

  <xsl:output method="text" />
  <xsl:variable name="newline">
    <xsl:text xml:space="preserve">
</xsl:text>
  </xsl:variable>

  <xsl:template match="d:PacketDefinitions">
    <xsl:variable name="xmlFileName" select="tokenize((tokenize(document-uri(/), '/')[position() = last()]), '[.]')[1]" />
    <xsl:text expand-text="yes"># {$xmlFileName}</xsl:text>
    <xsl:value-of select="$newline" />
    <xsl:value-of select="$newline" />
    <xsl:apply-templates select="d:Packets/d:Packet">
      <xsl:sort select="d:Code" data-type="text" />
      <xsl:sort select="d:SubCode" data-type="text" />
    </xsl:apply-templates>
  </xsl:template>
    
  <xsl:template match="d:Packet">
    <xsl:variable name="headerType" select="substring(d:HeaderType, 1, 2)" />
    <xsl:variable name="designation">
      <xsl:value-of select="substring(d:HeaderType, 1, 2)" />
      <xsl:text> </xsl:text>
      <xsl:value-of select="d:Code" />
      <xsl:text> </xsl:text>
      <xsl:if test="d:SubCode">
        <xsl:value-of select="d:SubCode" />
        <xsl:text> </xsl:text>
      </xsl:if>
      <xsl:text>- </xsl:text>
      <xsl:value-of select="d:Name" />
      <xsl:choose>
        <xsl:when test="d:Direction = 'ClientToServer'">
          <xsl:text> (by client)</xsl:text>
        </xsl:when>
        <xsl:when test="d:Direction = 'ServerToClient'">
          <xsl:text> (by server)</xsl:text>
        </xsl:when>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="fileName">
      <xsl:value-of select="substring(d:HeaderType, 1, 2)" />
      <xsl:text>-</xsl:text>
      <xsl:value-of select="d:Code" />
      <xsl:text>-</xsl:text>
      <xsl:if test="d:SubCode">
        <xsl:value-of select="d:SubCode" />
        <xsl:text>-</xsl:text>
      </xsl:if>
      <xsl:value-of select="d:Name" />
      <xsl:choose>
        <xsl:when test="d:Direction = 'ClientToServer'">
          <xsl:text>_by-client</xsl:text>
        </xsl:when>
        <xsl:when test="d:Direction = 'ServerToClient'">
          <xsl:text>_by-server</xsl:text>
        </xsl:when>
      </xsl:choose>
      <xsl:text>.md</xsl:text>
    </xsl:variable>
    <xsl:text expand-text="yes">  * [{$designation}]({encode-for-uri($fileName)})</xsl:text>
    <xsl:value-of select="$newline"/>
    <xsl:result-document href="{$fileName}" method="text">
      <xsl:text># </xsl:text>
      <xsl:value-of select="$designation" />
      <xsl:value-of select="$newline" />
      <xsl:value-of select="$newline" />

      <xsl:text>## Is sent when</xsl:text>
      <xsl:value-of select="$newline" />
      <xsl:value-of select="$newline" />
      <xsl:value-of select="d:SentWhen" />
      <xsl:value-of select="$newline" />
      <xsl:value-of select="$newline" />

      <xsl:text>## Causes the following actions on the </xsl:text>
      <xsl:choose>
        <xsl:when test="d:Direction = 'ClientToServer'">server side</xsl:when>
        <xsl:otherwise>client side</xsl:otherwise>
      </xsl:choose>
      <xsl:value-of select="$newline" />
      <xsl:value-of select="$newline" />
      <xsl:value-of select="d:CausedReaction" />
      <xsl:value-of select="$newline" />
      <xsl:value-of select="$newline" />

      <xsl:text>## Structure</xsl:text>
      <xsl:value-of select="$newline" />
      <xsl:value-of select="$newline" />
      <xsl:text>| Index | Length | Data Type | Value | Description |</xsl:text>
      <xsl:value-of select="$newline" />
      <xsl:text>|-------|--------|-----------|-------|-------------|</xsl:text>
      <xsl:value-of select="$newline" />
      <xsl:apply-templates select="d:HeaderType"/>
      <xsl:call-template name="PrintLength"/>
      <xsl:apply-templates select="d:Code"/>
      <xsl:apply-templates select="d:SubCode"/>
      <xsl:apply-templates select="d:Fields/d:Field"/>

      <xsl:apply-templates select="d:Structures/d:Structure" />
      <xsl:call-template name="CommonStructures"/>
      <xsl:apply-templates select="d:Enums/d:Enum" />
      <xsl:call-template name="CommonEnums"/>
    </xsl:result-document>
  </xsl:template>

  <xsl:template match="d:HeaderType[substring(.,1,2)='C1']">
    <xsl:text>| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |</xsl:text>
  </xsl:template>
  <xsl:template match="d:HeaderType[substring(.,1,2)='C2']">
    <xsl:text>| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |</xsl:text>
  </xsl:template>
  <xsl:template match="d:HeaderType[substring(.,1,2)='C3']">
    <xsl:text>| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |</xsl:text>
  </xsl:template>
  <xsl:template match="d:HeaderType[substring(.,1,2)='C4']">
    <xsl:text>| 0 | 1 |   Byte   | 0xC4  | [Packet type](PacketTypes.md) |</xsl:text>
  </xsl:template>
  
  <xsl:template name="PrintLength">
    <xsl:variable name="headerType" select="substring(d:HeaderType, 1, 2)" />
    <xsl:variable name="dataType">
      <xsl:choose>
        <xsl:when test="$headerType='C1' or $headerType='C3'">Byte</xsl:when>
        <xsl:otherwise>Short</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="length">
      <xsl:choose>
        <xsl:when test="$headerType='C1' or $headerType='C3'">1</xsl:when>
        <xsl:otherwise>2</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:value-of select="concat($newline, '| 1 | ',$length,' |    ', $dataType,'   |   ', d:Length, '   | Packet header - length of the packet |')"/>
  </xsl:template>
  <xsl:template match="d:Code">
    <xsl:variable name="headerType" select="substring(../d:HeaderType, 1, 2)" />
    <xsl:variable name="index">
      <xsl:choose>
        <xsl:when test="$headerType='C1' or $headerType='C3'">2</xsl:when>
        <xsl:otherwise>3</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:value-of select="concat($newline, '| ', $index, ' | 1 |    Byte   | 0x', ., '  | Packet header - packet type identifier |')"/>
  </xsl:template>

  <xsl:template match="d:SubCode">
    <xsl:variable name="headerType" select="substring(../d:HeaderType, 1, 2)" />
    <xsl:variable name="index">
      <xsl:choose>
        <xsl:when test="$headerType='C1' or $headerType='C3'">3</xsl:when>
        <xsl:otherwise>4</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:value-of select="concat($newline, '| ', $index, ' | 1 |    Byte   | 0x', ., '  | Packet header - sub packet type identifier |')"/>
  </xsl:template>

  <xsl:template match="d:Field">
    <xsl:value-of select="concat($newline, '| ')" />
    <xsl:apply-templates mode="index" />
    <xsl:text> | </xsl:text>
    <xsl:apply-templates mode="length" />
    <xsl:text> | </xsl:text>
    <xsl:apply-templates mode="type" />
    <xsl:value-of select="concat(' | ', d:DefaultValue , ' | ', d:Name)" />
    <xsl:if test="d:Description">
      <xsl:value-of select="concat('; ', d:Description)" />
    </xsl:if>
    <xsl:text> |</xsl:text>
  </xsl:template>

  <xsl:template match="d:Structure">
    <xsl:value-of select="$newline" />
    <xsl:value-of select="$newline" />
    <xsl:text>### </xsl:text>
    <xsl:value-of select="d:Name" />
    <xsl:text> Structure</xsl:text>
    <xsl:value-of select="$newline" />
    <xsl:value-of select="$newline" />
    <xsl:value-of select="d:Description" />
    <xsl:value-of select="$newline" />
    <xsl:value-of select="$newline" />

    <xsl:if test="d:Length">
      <xsl:text>Length: </xsl:text>
      <xsl:value-of select="d:Length"/>
      <xsl:text> Bytes</xsl:text>
      <xsl:value-of select="$newline" />
      <xsl:value-of select="$newline" />
    </xsl:if>

    <xsl:text>| Index | Length | Data Type | Value | Description |</xsl:text>
    <xsl:value-of select="$newline" />
    <xsl:text>|-------|--------|-----------|-------|-------------|</xsl:text>
    <xsl:apply-templates select="d:Fields/d:Field"/>

    <xsl:call-template name="CommonStructures"/>
    <xsl:call-template name="CommonEnums"/>
  </xsl:template>

  <xsl:template match="d:Enum">
    <xsl:value-of select="$newline" />
    <xsl:value-of select="$newline" />
    <xsl:text>### </xsl:text>
    <xsl:value-of select="d:Name" />
    <xsl:text> Enum</xsl:text>
    <xsl:value-of select="$newline" />
    <xsl:value-of select="$newline" />
    <xsl:value-of select="d:Description" />
    <xsl:value-of select="$newline" />
    <xsl:value-of select="$newline" />
    <xsl:text>| Value | Name | Description |</xsl:text>
    <xsl:value-of select="$newline" />
    <xsl:text>|-------|------|-------------|</xsl:text>
    <xsl:apply-templates select="d:Values/d:EnumValue" />
  </xsl:template>

  <xsl:template match="d:EnumValue">
    <xsl:value-of select="$newline" />
    <xsl:text>| </xsl:text>
    <xsl:value-of select="d:Value"/>
    <xsl:text> | </xsl:text>
    <xsl:value-of select="d:Name"/>
    <xsl:text> | </xsl:text>
    <xsl:value-of select="d:Description"/>
    <xsl:text> |</xsl:text>
  </xsl:template>

  <xsl:template name="CommonEnums">
    <xsl:for-each select="d:Fields/d:Field[d:Type='Enum' and not(d:Enums[d:Name = d:TypeName])]">
      <xsl:variable name="typeName" select="d:TypeName" />

      <xsl:if test="not(preceding-sibling::d:Field[d:TypeName = $typeName])">
        <xsl:apply-templates select="/d:PacketDefinitions/d:Enums/d:Enum[d:Name = $typeName]" />
        <xsl:apply-templates select="document('CommonEnums.xml')/d:PacketDefinitions/d:Enums/d:Enum[d:Name = $typeName]" />
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="CommonStructures">
    <xsl:for-each select="d:Fields/d:Field[d:Type='Structure[]' and not(d:Structures/d:Structure[d:Name = d:TypeName])]">
      <xsl:variable name="typeName" select="d:TypeName" />
      <xsl:if test="not(preceding-sibling::d:Field[d:TypeName = $typeName])">
        <xsl:apply-templates select="/d:PacketDefinitions/d:Structures/d:Structure[d:Name = $typeName]" />
        <xsl:apply-templates select="document('CommonEnums.xml')/d:PacketDefinitions/d:Structures/d:Structure[d:Name = $typeName]" />
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <!-- Index mapping: -->
  <xsl:template match="d:Type[. = 'Boolean' or . = 'Enum']" mode="index"> 
      <xsl:value-of select="../d:Index" />
      <xsl:if test="../d:LeftShifted">
        <xsl:text> &lt;&lt; </xsl:text>
        <xsl:value-of select="../d:LeftShifted"/>
      </xsl:if>
  </xsl:template>
  <xsl:template match="d:Type" mode="index">
    <xsl:value-of select="../d:Index" />
  </xsl:template>
  <xsl:template match="node()" mode="index">
    <xsl:apply-templates />
  </xsl:template>

  <!-- Type mapping: -->
  <xsl:template match="d:Type[. = 'Structure[]']" mode="type">
    <xsl:value-of select="concat('Array of ', ../d:TypeName)" />
  </xsl:template>
  <xsl:template match="d:Type[. = 'Enum']" mode="type">
    <xsl:value-of select="../d:TypeName" />
  </xsl:template>
  <xsl:template match="d:Type" mode="type">
    <xsl:value-of select="." />
  </xsl:template>
  <xsl:template match="node()" mode="type">
    <xsl:apply-templates />
  </xsl:template>

  <!-- Length mapping: -->
  <xsl:template match="d:Type[. = 'Boolean']" mode="length">
    <xsl:choose>
      <xsl:when test="../d:LeftShifted">1 bit</xsl:when>
      <xsl:otherwise>1</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="d:Type[. = 'Byte' or . = 'Enum']" mode="length">
    <xsl:choose>
      <xsl:when test="../d:Length">
        <xsl:value-of select="../d:Length" />
        <xsl:text> bit</xsl:text>
      </xsl:when>
      <xsl:otherwise>1</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template match="d:Type[. = 'ShortLittleEndian' or . = 'ShortBigEndian']" mode="length">2</xsl:template>
  <xsl:template match="d:Type[. = 'IntegerLittleEndian' or . = 'IntegerBigEndian']" mode="length">4</xsl:template>
  <xsl:template match="d:Type[. = 'LongLittleEndian' or . = 'LongBigEndian']" mode="length">8</xsl:template>
  <xsl:template match="d:Type[. = 'Float']" mode="length">4</xsl:template>
  <xsl:template match="d:Type[. = 'String']" mode="length">
    <xsl:value-of select="../d:Length"/>
  </xsl:template>
  <xsl:template match="d:Type[. = 'Binary']" mode="length">
    <xsl:value-of select="../d:Length"/>
  </xsl:template>
  <xsl:template match="d:Type[. = 'Structure[]']" mode="length">
    <xsl:value-of select="../d:TypeName"/>
    <xsl:text>.Length * </xsl:text>
    <xsl:value-of select="../d:ItemCountField"/>
  </xsl:template>
  <xsl:template match="node()" mode="length">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="node()">
    <xsl:apply-templates />
  </xsl:template>
</xsl:stylesheet>